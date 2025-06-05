using DurableTask.Core;

namespace Api.Domain.DomainServices;

// temp fix
public class SimpleObjectCreator<T> : ObjectCreator<T>
{
    readonly T obj;

    public SimpleObjectCreator(string name, T obj)
        : this(name, string.Empty, obj)
    {
    }

    public SimpleObjectCreator(string name, string version, T obj)
    {
        this.Name = name;
        this.Version = version;
        this.obj = obj;
    }

    public override T Create() => this.obj;
}
