using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public struct AnimationEventMessage
    {
        public string EventKey { get; }
        public float FloatValue { get; }
        public int IntValue { get; }
        public string StringValue { get; }
        public Object ObjectValue { get; }

        public AnimationEventMessage(string eventKey, float floatValue = 0f, int intValue = 0,
                string stringValue = null, Object objectValue = null)
        {
            EventKey = eventKey;
            FloatValue = floatValue;
            IntValue = intValue;
            StringValue = stringValue;
            ObjectValue = objectValue;
        }
    }

    public class AnimationEventHandler : MonoBehaviour
    {
        public void OnAnimationEvent(string eventKey)
        {
            MessageBroker.Default.Publish(new AnimationEventMessage(eventKey));
        }

        public void OnAnimationEventFloat(float value)
        {
            MessageBroker.Default.Publish(new AnimationEventMessage("FloatEvent", value));
        }

        public void OnAnimationEventString(string value)
        {
            var parts = value.Split('.');
            if (parts.Length > 1)
            {
                MessageBroker.Default.Publish(new AnimationEventMessage(parts[0], stringValue: parts[1]));
            }
            else
            {
                MessageBroker.Default.Publish(new AnimationEventMessage("StringEvent", stringValue: value));
            }
        }

        public void OnAnimationEventObject(Object value)
        {
            MessageBroker.Default.Publish(new AnimationEventMessage("ObjectEvent", objectValue: value));
        }
    }
}
