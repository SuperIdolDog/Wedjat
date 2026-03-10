using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Helper
{
    public class EventBus
    {
    //    // 存储事件类型与订阅者委托的映射（线程安全字典）
    //private static readonly ConcurrentDictionary<Type, ConcurrentBag<Delegate>> _eventHandlers = new ConcurrentDictionary<Type, ConcurrentBag<Delegate>>();

    //    /// <summary>
    //    /// 订阅事件
    //    /// </summary>
    //    /// <typeparam name="TEventArgs">事件参数类型（必须继承EventArgs）</typeparam>
    //    /// <param name="handler">事件处理委托</param>
    //    public static void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
    //    {
    //        if (handler == null) throw new ArgumentNullException(nameof(handler));

    //        var eventType = typeof(TEventArgs);
    //        // 不存在则创建对应事件的委托集合
    //        _eventHandlers.TryAdd(eventType, new ConcurrentBag<Delegate>());
    //        _eventHandlers[eventType].Add(handler);
    //    }

    //    /// <summary>
    //    /// 取消订阅事件
    //    /// </summary>
    //    /// <typeparam name="TEventArgs">事件参数类型</typeparam>
    //    /// <param name="handler">要取消的事件处理委托</param>
    //    public static void Unsubscribe<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
    //    {
    //        if (handler == null) throw new ArgumentNullException(nameof(handler));

    //        var eventType = typeof(TEventArgs);
    //        if (_eventHandlers.TryGetValue(eventType, out var handlers))
    //        {
    //            // 移除指定委托（线程安全）
    //            var newHandlers = handlers.Where(h => h != handler).ToList();
    //            _eventHandlers[eventType] = new ConcurrentBag<Delegate>(newHandlers);

    //            // 若没有订阅者，移除该事件类型以释放资源
    //            if (_eventHandlers[eventType].IsEmpty)
    //                _eventHandlers.TryRemove(eventType, out _);
    //        }
    //    }

    //    /// <summary>
    //    /// 发布事件
    //    /// </summary>
    //    /// <typeparam name="TEventArgs">事件参数类型</typeparam>
    //    /// <param name="sender">事件发布者</param>
    //    /// <param name="eventArgs">事件参数</param>
    //    public static void Publish<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
    //    {
    //        if (sender == null) throw new ArgumentNullException(nameof(sender));
    //        if (eventArgs == null) throw new ArgumentNullException(nameof(eventArgs));

    //        var eventType = typeof(TEventArgs);
    //        // 存在订阅者则触发所有委托
    //        if (_eventHandlers.TryGetValue(eventType, out var handlers))
    //        {
    //            // 复制一份委托集合，避免发布时取消订阅导致异常
    //            var handlersCopy = handlers.ToArray();
    //            foreach (var handler in handlersCopy)
    //            {
    //                (handler as EventHandler<TEventArgs>)?.Invoke(sender, eventArgs);
    //            }
    //        }
    //    }
    }
}
