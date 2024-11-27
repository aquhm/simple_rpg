namespace Client.Actor
{
    public interface IAnimationHandler
    {
        void SetBool(string name, bool value);
        void SetTrigger(string name);
        void SetInteger(string name, int value);
        void SetLayerWeight(int layerIndex, float weight);
        void SetFloat(string name, float value);
    }
}