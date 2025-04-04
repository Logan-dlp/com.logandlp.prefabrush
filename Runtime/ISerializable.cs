namespace com.logandlp.prefabrush.runtime
{
    public interface ISerializable<T>
    {
        T Serialize();
        void Deserialize(T component);
    }
}