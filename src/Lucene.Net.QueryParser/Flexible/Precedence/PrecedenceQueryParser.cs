﻿using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Flexible.Precedence.Processors;
using Lucene.Net.QueryParsers.Flexible.Standard;

namespace Lucene.Net.QueryParsers.Flexible.Precedence
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

    /// <summary>
    /// This query parser works exactly as the standard query parser ( {@link StandardQueryParser} ), 
    /// except that it respect the boolean precedence, so &lt;a AND b OR c AND d&gt; is parsed to &lt;(+a +b) (+c +d)&gt;
    /// instead of &lt;+a +b +c +d&gt;.
    /// <para>
    /// EXPERT: This class extends {@link StandardQueryParser}, but uses {@link PrecedenceQueryNodeProcessorPipeline}
    /// instead of {@link StandardQueryNodeProcessorPipeline} to process the query tree.
    /// </para>
    /// </summary>
    /// <seealso cref="StandardQueryParser"/>
    public class PrecedenceQueryParser : StandardQueryParser
    {
        /**
   * @see StandardQueryParser#StandardQueryParser()
   */
        public PrecedenceQueryParser()
        {
            SetQueryNodeProcessor(new PrecedenceQueryNodeProcessorPipeline(QueryConfigHandler));
        }

        /**
         * @see StandardQueryParser#StandardQueryParser(Analyzer)
         */
        public PrecedenceQueryParser(Analyzer analyer)
            : base(analyer)
        {
            SetQueryNodeProcessor(new PrecedenceQueryNodeProcessorPipeline(QueryConfigHandler));
        }
    }
}
