using System.Text.RegularExpressions;

namespace JsonTransform
{
    public class MatchStateMachine
    {
        private readonly State _initialState;

        public MatchStateMachine(string[] parts)
        {
            var current = _initialState = new State(parts[0], parts.Length == 1);

            for (var i = 1; i < parts.Length; ++i)
            {
                var next = new State(parts[i], i == parts.Length - 1);

                //Merge adjacent "any path" segments
                if (current.IsAnyPathSegment && next.IsAnyPathSegment)
                {
                    next.PreviousState = current.PreviousState;

                    if (current.PreviousState == null)
                    {
                        _initialState = next;
                    }
                    else
                    {
                        current.PreviousState.NextState = next;
                        current = next;
                    }

                    continue;
                }
                
                current.NextState = next;
                next.PreviousState = current;
                current = next;
            }
        }

        private static bool IsMatchFromPoint(string[] parts, int index, State current)
        {
            for (var i = index; i < parts.Length; ++i)
            {
                if (!current.IsMatch(parts[i]))
                {
                    return false;
                }

                if (current.IsAnyPathSegment)
                {
                    if (current.IsLeafNode)
                    {
                        return true;
                    }

                    //See if we can process the post-span conditions (** is 0 or more so start trying at 'i')
                    for (var j = i; j < parts.Length; ++j)
                    {
                        if (IsMatchFromPoint(parts, j, current.NextState))
                        {
                            return true;
                        }
                    }

                    //If nothing matched, nothing can match
                    return false;
                }

                if (current.IsLeafNode)
                {
                    return true;
                }

                current = current.NextState;
            }

            return false;
        }

        public bool IsMatch(string[] parts)
        {
            return IsMatchFromPoint(parts, 0, _initialState);
        }

        public class State
        {
            public bool IsLeafNode { get; private set; }

            public bool IsAnyPathSegment { get; private set; }

            public string Pattern { get; private set; }

            public State NextState { get; set; }
            
            public State PreviousState { get; set; }

            public State(string pattern, bool isLeaf)
            {
                IsLeafNode = isLeaf;
                Pattern = pattern;

                if (pattern == "**")
                {
                    IsAnyPathSegment = true;
                }
            }

            public bool IsMatch(string part)
            {
                if (IsAnyPathSegment)
                {
                    return true;
                }

                return Regex.IsMatch(part, Pattern);
            }
        }
    }
}