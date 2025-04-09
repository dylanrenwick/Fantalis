using Arch.Core;

namespace Fantalis.Core.Systems;

public interface ISystem
{
    public World World { get; set; }
    
    public void Initialize();

    public void BeforeUpdate(double deltaTime);
    public void Update(double deltaTime);
    public void AfterUpdate(double deltaTime);
}