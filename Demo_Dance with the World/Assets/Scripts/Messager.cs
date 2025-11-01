using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagTypesChangedMessage {
    public readonly Stack<E_MagMode> Types;

    public MagTypesChangedMessage(Stack<E_MagMode> types) {
        Types = types;
    }
}

public class LevelResetMessage {
    public readonly int LevelId;

    public LevelResetMessage(int levelId) {
        LevelId = levelId;
    }
}

public class CheckPointMessage {
    public readonly int LevelId;

    public CheckPointMessage(int levelId) {
        LevelId = levelId;
    }
}

public class PlayerNeedResetMessage {
    public readonly int LevelId;
    public readonly Rigidbody PlayerRigidbody;
    public readonly Transform PlayerTransform;
    public readonly PlayerMag PlayerMagComponent;
    public readonly bool IsRebirth;

    public PlayerNeedResetMessage(int levelId, Rigidbody playerRigidbody, Transform playerTransform,
        PlayerMag playerMagComponent, bool isRebirth) {
        LevelId = levelId;
        PlayerRigidbody = playerRigidbody;
        PlayerTransform = playerTransform;
        PlayerMagComponent = playerMagComponent;
        IsRebirth = isRebirth;
    }
}

public class DisplayMessage {
    public readonly string Message;
    public readonly float Duration;

    public DisplayMessage(string message, float duration) {
        Message = message;
        Duration = duration;
    }
}

public class SparkMessage {
}

public class Messager : MonoBehaviour {
    private class MessageReceiver {
        public object Receiver { get; }
        public Action<object> Handler { get; }

        public MessageReceiver(object receiver, Action<object> handler) {
            Receiver = receiver;
            Handler = handler;
        }
    }

    private static readonly Dictionary<Type, List<MessageReceiver>> Events = new();

    public static void Register<TMessage>(object receiver, Action<TMessage> handler) {
        var type = typeof(TMessage);
        if (!Events.ContainsKey(type)) {
            Events[type] = new List<MessageReceiver>();
        }

        Events[type].Add(new MessageReceiver(receiver, o => handler((TMessage)o)));
    }

    public static void Send<TMessage>(TMessage message) {
        var type = typeof(TMessage);
        if (!Events.TryGetValue(type, out var handlers)) {
            return;
        }

        foreach (var receiver in handlers) {
            receiver.Handler.Invoke(message);
        }
    }
}