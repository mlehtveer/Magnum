// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Pipeline
{
    using System.Collections.Generic;
    using Segments;
    using Visitors;

    public class SegmentUnbinder :
        AbstractPipeVisitor
    {
        private readonly HashSet<Pipe> _segmentsToUnbind;

        public SegmentUnbinder(IEnumerable<Pipe> segmentsToUnbind)
        {
            _segmentsToUnbind = new HashSet<Pipe>(segmentsToUnbind);
        }

        protected override RecipientListSegment VisitRecipientList(RecipientListSegment recipientList)
        {
            if (recipientList == null)
                return null;

            bool modified = false;
            IList<Pipe> recipients = new List<Pipe>();

            foreach (Pipe recipient in recipientList.Recipients)
            {
                if (_segmentsToUnbind.Contains(recipient))
                {
                    modified = true;
                    continue;
                }

                Pipe result = Visit(recipient);
                if (result != recipient)
                {
                    modified = true;
                }

                if (result != null)
                    recipients.Add(result);
            }

            if (modified)
            {
                return new RecipientListSegment(recipientList.MessageType, recipients);
            }

            return recipientList;
        }

        public void RemoveFrom(Pipe pipe)
        {
            base.Visit(pipe);
        }
    }
}