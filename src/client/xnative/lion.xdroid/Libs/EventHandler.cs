using System;

namespace Lion.XDroid.Libs
{
    public class PositionEventArgs : EventArgs
    {
        public PositionEventArgs(int position)
        {
            Position = position;
        }

        public int Position { get; set; }
    }

    public class DialogEventArgs : EventArgs
    {
        public DialogEventArgs(bool cancel)
        {
            Cancel = cancel;
        }

        public bool Cancel { get; set; }
    }

    public class RefreshEventArgs : EventArgs
    {
        public RefreshEventArgs(bool refresh)
        {
            Refresh = refresh;
        }

        public bool Refresh { get; set; }
    }

    public class FinishEventArgs : EventArgs
    {
        public FinishEventArgs(string json)
        {
            Json = json;
        }

        public string Json { get; set; }
    }
}