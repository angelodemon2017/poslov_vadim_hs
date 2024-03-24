
public interface IHaveModel
{
    void Init<T>(T baseModel) where T : IBaseModel;
}