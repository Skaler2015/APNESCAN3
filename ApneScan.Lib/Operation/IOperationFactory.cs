namespace ApneScan.Operation;

public interface IOperationFactory
{
    T Create<T>() where T : IOperation;
}