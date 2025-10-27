using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messager : MonoBehaviour {
    class MessageReceiver {
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