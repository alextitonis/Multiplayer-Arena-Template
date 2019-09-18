using System.Collections.Generic;

namespace GameServer
{
    public class InputBuffer
    {
        /*
        _Input[] buffer;

        public InputBuffer(int length)
        {
            buffer = new _Input[length];
        }

        public void Add(float horizontal, float vertical)
        {
            bool added = false;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == null)
                {
                    buffer[i] = new _Input(horizontal, vertical);
                    added = true;
                    break;
                }
            }

            if (!added)
                Server.getInstance.Log("Buffer (Add) passed over limits, is already full!", LogType.Warning);
        }
        public _Input Get()
        {
            _Input input = null;

            if (buffer[0] != null)
            {
                input = buffer[0];
                Remove(0);
            }

            if (input == null)
            {
                Server.getInstance.Log("Index[0] was not empty for a byte buffer!", LogType.Warning);

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != null)
                    {
                        input = buffer[i];
                        Remove(i);
                        break;
                    }
                }
            }

            return input;
        }
        public bool hasInput
        {
            get
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != null)
                        return true;
                }

                return false;
            }
        }

        void Remove(int index)
        {
            buffer[index] = null;
            for (int i = index; i < buffer.Length; i++)
            {
                buffer[i - 1] = buffer[i];
            }
        }
        */
        LinkedList<_Input> buffer;

        public InputBuffer()
        {
            buffer = new LinkedList<_Input>();
        }

        public void Add(float horizontal, float vertical, bool running)
        {
            buffer.AddLast(new _Input(horizontal, vertical, running));
        }
        public _Input Get()
        {
            _Input i = buffer.First.Value;
            buffer.RemoveFirst();
            return i;
        }
        public bool hasInput => buffer.Count > 0;
    }

    [System.Serializable]
    public class _Input
    {
        public float horizontal;
        public float vertical;
        public bool running;

        public _Input(float horizontal, float vertical, bool running)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
            this.running = running;
        }
    }
}