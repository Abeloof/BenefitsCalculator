using Api.Domain.Dtos.Employee;
using DurableTask.AzureStorage;
using DurableTask.Core;
using Newtonsoft.Json;

namespace Api.Domain.DomainServices
{
    public static class TempTaskHubClientExtensions
    {

        public async static Task<OrchestrationState> GetOrCreateOrchestration(this TaskHubClient orchestrationClient, GetEmployeeDto employee,
                            int earningPeriodId, CancellationToken cancellationToken = default)
        {
            var instanceId = $"{employee.Id}-{earningPeriodId}-{employee.GetHashCode()}";
            var exists = await orchestrationClient.GetOrchestrationStateAsync(instanceId);

            async Task<OrchestrationState> RetryFailedOrTerminatedOrCanceledAndWait()
            {
                var service = (AzureStorageOrchestrationService)orchestrationClient.ServiceClient;
                await service.RewindTaskOrchestrationAsync(instanceId, "restarted per request");
                return await orchestrationClient.WaitForOrchestrationAsync(new OrchestrationInstance
                {
                    InstanceId = instanceId
                }, TimeSpan.FromSeconds(30), cancellationToken);
            };

            async Task<OrchestrationState> ResumeSuspendedAndWait()
            {
                var service = (AzureStorageOrchestrationService)orchestrationClient.ServiceClient;
                await orchestrationClient.ResumeInstanceAsync(new OrchestrationInstance
                {
                    InstanceId = instanceId
                }, "resume per request");
                return await orchestrationClient.WaitForOrchestrationAsync(new OrchestrationInstance
                {
                    InstanceId = instanceId
                }, TimeSpan.FromSeconds(30), cancellationToken);
            };
            async Task<OrchestrationState> CreateNewAndWait()
            {
                var instance = await orchestrationClient //TODO: Demo only
                    .CreateOrchestrationInstanceAsync(nameof(EarningsCalulator), string.Empty, instanceId, (earningPeriodId, employee));
                return await orchestrationClient.WaitForOrchestrationAsync(instance, TimeSpan.FromSeconds(30), cancellationToken);
            };
            return exists?.OrchestrationStatus switch
            {
                OrchestrationStatus.Completed => exists,
                OrchestrationStatus.Failed => await RetryFailedOrTerminatedOrCanceledAndWait(),
                OrchestrationStatus.Terminated => await RetryFailedOrTerminatedOrCanceledAndWait(),
                OrchestrationStatus.Running => await orchestrationClient
                                                        .WaitForOrchestrationAsync(new OrchestrationInstance
                                                        {
                                                            InstanceId = instanceId
                                                        }, TimeSpan.FromSeconds(30), cancellationToken),
                OrchestrationStatus.ContinuedAsNew => await orchestrationClient
                                                        .WaitForOrchestrationAsync(new OrchestrationInstance
                                                        {
                                                            InstanceId = instanceId
                                                        }, TimeSpan.FromSeconds(30), cancellationToken),
                OrchestrationStatus.Canceled => await RetryFailedOrTerminatedOrCanceledAndWait(),
                OrchestrationStatus.Pending => await orchestrationClient
                                                        .WaitForOrchestrationAsync(new OrchestrationInstance
                                                        {
                                                            InstanceId = instanceId
                                                        }, TimeSpan.FromSeconds(30), cancellationToken),
                OrchestrationStatus.Suspended => await ResumeSuspendedAndWait(),
                null => await CreateNewAndWait(),
                _ => await CreateNewAndWait(),
            };
        }

        public static T? AsT<T>(this string? output)
        {
            if (output == null) return default;
            return JsonConvert.DeserializeObject<T>(output)!;
        }
    }    
}