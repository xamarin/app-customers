using System;
using FormsToolkit;

namespace Customers
{
    // This derived class could be eliminated from the project if @motz accepts my PR: https://github.com/jamesmontemagno/xamarin.forms-toolkit/pull/1
    public class MessagingServiceQuestionWithAction : MessagingServiceQuestion
    {
        public Action SuccessAction { get; set; }
    }
}

