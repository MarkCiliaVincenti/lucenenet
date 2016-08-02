﻿using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;
using System.Runtime.CompilerServices;

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


     // Create basic queries to be used during rewrite.
     // The basic queries are TermQuery and SpanTermQuery.
     // An exception can be thrown when too many of these are used.
     // SpanTermQuery and TermQuery use IndexReader.termEnum(Term), which causes the buffer usage.
     
     // Use this class to limit the buffer usage for reading terms from an index.
     // Default is 1024, the same as the max. number of subqueries for a BooleanQuery.



    /// <summary>
    /// Factory for creating basic term queries
    /// </summary>
    public class BasicQueryFactory
    {
        public BasicQueryFactory(int maxBasicQueries)
        {
            this.maxBasicQueries = maxBasicQueries;
            this.queriesMade = 0;
        }

        public BasicQueryFactory()
            : this(1024)
        {
        }

        private int maxBasicQueries;
        private int queriesMade;

        public int NrQueriesMade { get { return queriesMade; } }
        public int MaxBasicQueries { get { return maxBasicQueries; } }

        public override string ToString()
        {
            return GetType().Name
                + "(maxBasicQueries: " + maxBasicQueries
                + ", queriesMade: " + queriesMade
                + ")";
        }

        private bool AtMax
        {
            get { return queriesMade >= maxBasicQueries; }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void CheckMax()
        {
            if (AtMax)
                throw new TooManyBasicQueries(MaxBasicQueries);
            queriesMade++;
        }

        public TermQuery NewTermQuery(Term term)
        {
            CheckMax();
            return new TermQuery(term);
        }

        public SpanTermQuery NewSpanTermQuery(Term term)
        {
            CheckMax();
            return new SpanTermQuery(term);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ (AtMax ? 7 : 31 * 32);
        }

        /// <summary>
        /// Two BasicQueryFactory's are equal when they generate
        /// the same types of basic queries, or both cannot generate queries anymore.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BasicQueryFactory))
                return false;
            BasicQueryFactory other = (BasicQueryFactory)obj;
            return AtMax == other.AtMax;
        }
    }
}
