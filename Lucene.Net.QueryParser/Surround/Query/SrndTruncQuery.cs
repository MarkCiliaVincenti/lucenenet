﻿using Lucene.Net.Index;
using Lucene.Net.Util;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Lucene.Net.QueryParser.Surround.Query
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    public class SrndTruncQuery : SimpleTerm
    {
        public SrndTruncQuery(string truncated, char unlimited, char mask)
            : base(false) /* not quoted */
        {
            this.truncated = truncated;
            this.unlimited = unlimited;
            this.mask = mask;
            TruncatedToPrefixAndPattern();
        }

        private readonly string truncated;
        private readonly char unlimited;
        private readonly char mask;

        private string prefix;
        private BytesRef prefixRef;
        private Regex pattern;

        public virtual string Truncated { get { return truncated; } }

        public override string ToStringUnquoted()
        {
            return Truncated;
        }

        protected virtual bool MatchingChar(char c)
        {
            return (c != unlimited) && (c != mask);
        }

        protected virtual void AppendRegExpForChar(char c, StringBuilder re)
        {
            if (c == unlimited)
                re.Append(".*");
            else if (c == mask)
                re.Append(".");
            else
                re.Append(c);
        }

        protected virtual void TruncatedToPrefixAndPattern()
        {
            int i = 0;
            while ((i < truncated.Length) && MatchingChar(truncated[i]))
            {
                i++;
            }
            prefix = truncated.Substring(0, i);
            prefixRef = new BytesRef(prefix);

            StringBuilder re = new StringBuilder();
            while (i < truncated.Length)
            {
                AppendRegExpForChar(truncated[i], re);
                i++;
            }
            pattern = new Regex(re.ToString(), RegexOptions.Compiled);
        }

        // TODO: Finish implementation
        public override void VisitMatchingTerms(IndexReader reader, string fieldName, SimpleTerm.IMatchingTermVisitor mtv)
        {
            throw new NotImplementedException("Need to translate this from Java's whacky RegEx syntax");
            //int prefixLength = prefix.Length;
            //Terms terms = MultiFields.GetTerms(reader, fieldName);
            //if (terms != null)
            //{
            //    MatchCollection matcher = pattern.Matches("");
            //    try
            //    {
            //        TermsEnum termsEnum = terms.Iterator(null);

            //        TermsEnum.SeekStatus status = termsEnum.SeekCeil(prefixRef);
            //        BytesRef text;
            //        if (status == TermsEnum.SeekStatus.FOUND)
            //        {
            //            text = prefixRef;
            //        }
            //        else if (status == TermsEnum.SeekStatus.NOT_FOUND)
            //        {
            //            text = termsEnum.Term();
            //        }
            //        else
            //        {
            //            text = null;
            //        }

            //        while (text != null)
            //        {
            //            if (text != null && StringHelper.StartsWith(text, prefixRef))
            //            {
            //                string textString = text.Utf8ToString();
            //                matcher.Reset(textString.Substring(prefixLength));
            //                if (matcher.Success)
            //                {
            //                    mtv.VisitMatchingTerm(new Term(fieldName, textString));
            //                }
            //            }
            //            else
            //            {
            //                break;
            //            }
            //            text = termsEnum.Next();
            //        }
            //    }
            //    finally
            //    {
            //        matcher.Reset();
            //    }
            //}
        }
    }
}
