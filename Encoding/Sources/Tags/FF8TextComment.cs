using System;

namespace OpenVIII.Encoding.Tags
{
    public sealed class FF8TextComment
    {
        private static readonly String[] LineCommentEnd = {"\r\n", "\n", "{Line}"};
        private static readonly String[] BlockCommentEnd = {"*/"};

        public enum CommentType
        {
            Line = '/',
            Block = '*'
        }

        public readonly CommentType Type;
        public readonly String Value;

        private FF8TextComment(CommentType commentType, String value)
        {
            Type = commentType;
            Value = value;
        }

        public static FF8TextComment TryRead(Char[] chars, ref Int32 offset, ref Int32 left)
        {
            if (left < 2 || chars[offset] != '/')
                return null;

            var commentType = (CommentType)chars[offset + 1];
            if (commentType != CommentType.Line && commentType != CommentType.Block)
                return null;

            String value;
            var index = IndexOfAny(chars, offset + 2, left - 2, out var finded, commentType == CommentType.Line ? LineCommentEnd : BlockCommentEnd);

            if (index < 0)
            {
                value = new String(chars, offset + 2, left - 2);
                offset = offset + left;
                left = 0;
            }
            else
            {
                if (commentType == CommentType.Line)
                {
                    if (offset != 0 && IndexOfAny(chars, offset, -6, out var prev, LineCommentEnd) < 0)
                        finded = string.Empty;
                }

                var length = index - offset;
                value = new String(chars, offset + 2, length - 2);
                left -= length + finded.Length;
                offset = index + finded.Length;
            }

            return new FF8TextComment(commentType, value);
        }

        private static Int32 IndexOfAny(Char[] chars, Int32 offset, Int32 left, out String finded, params String[] subStrings)
        {
            var counters = new Int32[subStrings.Length];

            if (left > 0)
            {
                for (var i = offset; i < chars.Length && left > 0; i++, left--)
                {
                    for (var k = 0; k < subStrings.Length; k++)
                    {
                        var str = subStrings[k];
                        if (chars[i] != str[counters[k]++])
                            counters[k] = 0;
                        else if (counters[k] == str.Length)
                        {
                            finded = str;
                            return i - str.Length + 1;
                        }
                    }
                }
            }
            else
            {
                for (var i = offset; i >= 0 && left < 0; i--, left++)
                {
                    for (var k = 0; k < subStrings.Length; k++)
                    {
                        var str = subStrings[k];
                        if (chars[i] != str[str.Length - 1 - counters[k]++])
                            counters[k] = 0;
                        else if (counters[k] == str.Length)
                        {
                            finded = str;
                            return i - str.Length + 1;
                        }
                    }
                }
            }

            finded = null;
            return -1;
        }

    }
}