using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenUGD
{
    public interface ISignal
    {
        bool Subscribe(Lifetime lifetime, Action handler);
    }

    public class Signal : ISignal
    {
        private static readonly Stack<List<Action>> Pool = new Stack<List<Action>>();
        private readonly Lifetime _lifetime;
        private readonly object _lock = new object();
        private readonly List<Action> _sinks = new List<Action>(1);

        public Signal(Lifetime lifetime)
        {
            _lifetime = lifetime;
            if (lifetime == null)
            {
                throw new ArgumentNullException(nameof(lifetime));
            }

            lifetime.AddAction(() =>
            {
                lock (_sinks)
                {
                    _sinks.Clear();
                }
            });
        }

        public bool Subscribe(Lifetime lifetime, Action handler)
        {
            if (_lifetime.IsTerminated)
            {
                return false;
            }

            void Opening()
            {
                lock (_lock)
                {
                    if (_sinks.Any(action => ReferenceEquals(action, handler)))
                    {
                        throw new InvalidOperationException($"The handler “{handler}” is already sinking the signal.");
                    }

                    _sinks.Add(handler);
                }
            }

            void Closing()
            {
                lock (_lock)
                {
                    for (var index = 0; index < _sinks.Count; index++)
                    {
                        var action = _sinks[index];
                        if (!ReferenceEquals(action, handler)) continue;
                        _sinks.RemoveAt(index);
                        return;
                    }
                }
            }

            lifetime.AddBracket(Opening, Closing);
            return true;
        }

        public void Fire()
        {
            List<Action> copy;
            lock (Pool)
            {
                copy = Pool.Count != 0 ? Pool.Pop() : new List<Action>();
            }
            lock (_lock)
            {
                copy.AddRange(_sinks);
            }

            foreach (var action in copy)
            {
                action();
            }

            copy.Clear();
            lock (Pool)
            {
                Pool.Push(copy);
            }
        }
    }

    public interface ISignal<T>
    {
        bool Subscribe(Lifetime lifetime, Action<T> handler);
    }

    public class Signal<T1> : ISignal<T1>
    {
        private static readonly Stack<List<Action<T1>>> Pool = new Stack<List<Action<T1>>>();
        private readonly Lifetime _lifetime;
        private readonly object _lock = new object();
        private readonly List<Action<T1>> _sinks = new List<Action<T1>>(1);

        public Signal(Lifetime lifetime)
        {
            _lifetime = lifetime;
            if (lifetime == null)
            {
                throw new ArgumentNullException(nameof(lifetime));
            }

            lifetime.AddAction(() =>
            {
                lock (_sinks)
                {
                    _sinks.Clear();
                }
            });
        }

        public bool Subscribe(Lifetime lifetime, Action<T1> handler)
        {
            if (_lifetime.IsTerminated)
            {
                return false;
            }

            void Opening()
            {
                lock (_lock)
                {
                    if (_sinks.Any(action => ReferenceEquals(action, handler)))
                    {
                        throw new InvalidOperationException($"The handler “{handler}” is already sinking the signal.");
                    }

                    _sinks.Add(handler);
                }
            }

            void Closing()
            {
                lock (_lock)
                {
                    for (var index = 0; index < _sinks.Count; index++)
                    {
                        var action = _sinks[index];
                        if (!ReferenceEquals(action, handler)) continue;
                        _sinks.RemoveAt(index);
                        return;
                    }
                }
            }

            lifetime.AddBracket(Opening, Closing);
            return true;
        }

        public void Fire(T1 value)
        {
            List<Action<T1>> copy;
            lock (Pool)
            {
                copy = Pool.Count != 0 ? Pool.Pop() : new List<Action<T1>>();
            }
            lock (_lock)
            {
                copy.AddRange(_sinks);
            }

            foreach (var action in copy)
            {
                action(value);
            }

            copy.Clear();
            lock (Pool)
            {
                Pool.Push(copy);
            }
        }
    }

    public interface ISignal<T1, T2>
    {
        bool Subscribe(Lifetime lifetime, Action<T1, T2> handler);
    }

    public class Signal<T1, T2> : ISignal<T1, T2>
    {
        private static readonly Stack<List<Action<T1, T2>>> Pool = new Stack<List<Action<T1, T2>>>();
        private readonly Lifetime _lifetime;
        private readonly object _lock = new object();
        private readonly List<Action<T1, T2>> _sinks = new List<Action<T1, T2>>(1);

        public Signal(Lifetime lifetime)
        {
            _lifetime = lifetime;
            if (lifetime == null)
            {
                throw new ArgumentNullException(nameof(lifetime));
            }

            lifetime.AddAction(() =>
            {
                lock (_sinks)
                {
                    _sinks.Clear();
                }
            });
        }

        public bool Subscribe(Lifetime lifetime, Action<T1, T2> handler)
        {
            if (_lifetime.IsTerminated)
            {
                return false;
            }

            void Opening()
            {
                lock (_lock)
                {
                    if (_sinks.Any(action => ReferenceEquals(action, handler)))
                    {
                        throw new InvalidOperationException($"The handler “{handler}” is already sinking the signal.");
                    }

                    _sinks.Add(handler);
                }
            }

            void Closing()
            {
                lock (_lock)
                {
                    for (var index = 0; index < _sinks.Count; index++)
                    {
                        var action = _sinks[index];
                        if (!ReferenceEquals(action, handler)) continue;
                        _sinks.RemoveAt(index);
                        return;
                    }
                }
            }

            lifetime.AddBracket(Opening, Closing);
            return true;
        }

        public void Fire(T1 value, T2 value2)
        {
            List<Action<T1, T2>> copy;
            lock (Pool)
            {
                copy = Pool.Count != 0 ? Pool.Pop() : new List<Action<T1, T2>>();
            }
            lock (_lock)
            {
                copy.AddRange(_sinks);
            }

            foreach (var action in copy)
            {
                action(value, value2);
            }

            copy.Clear();
            lock (Pool)
            {
                Pool.Push(copy);
            }
        }
    }
}

