using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentViewer
{

    [Serializable]
    public class BugException : Exception
    {
        public string Details { get; set; }
        public BugException(string message) : base(message) { }
        public BugException(string message, Exception inner) : base(message, inner) { }
        protected BugException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
