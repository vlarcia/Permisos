using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.Utilidades
{
    class ChecklistMessage : ValueChangedMessage<ChecklistResult>
    {
        public ChecklistMessage(ChecklistResult value) : base(value)
        {

        }
    }
}
