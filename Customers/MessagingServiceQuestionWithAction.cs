using System;
using FormsToolkit;

namespace Customers
{
    public class MessagingServiceQuestionWithAction : MessagingServiceQuestion
    {
        public Action SuccessAction { get; set; }
    }
}

