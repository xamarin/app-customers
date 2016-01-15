using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Customers.UITests
{
    [TestFixture(Platform.iOS)]
    public class iOSTests
    {
        IApp app;
        Platform platform;

        public iOSTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void EditCustomer ()
        {
            app.Tap(x => x.Marked("Armstead, Evan"));
            app.Screenshot("Selected contact with name: 'Armstead, Evan'");
            app.Tap(x => x.Marked("edit"));
            app.Screenshot("Tapped on 'edit' button");
            app.Tap(x => x.Marked("First").Sibling());
            app.ClearText(x => x.Marked("First").Sibling());
            app.EnterText("Edited");
            app.Screenshot("Entered 'Edited' into First Name Field");
            app.Tap(x => x.Marked("Last").Sibling());
            app.ClearText(x => x.Marked("Last").Sibling());
            app.EnterText("Name");
            app.Screenshot("Entered 'Name' into Last Name Field");
            app.Tap(x => x.Id("save"));
            app.Screenshot("Saved contact");
            app.WaitForElement(x => x.Text("Edited Name"));
            app.Screenshot("Verified contacts name changed to: 'Edited Name'");
        }

        [Test]
        public void EditCustomerInList ()
        {
            app.Tap(x => x.Marked("Cardell, Jesus"));
            app.Screenshot("Selected contact with name: 'Cardell, Jesus'");
            app.Tap(x => x.Marked("edit"));
            app.Screenshot("Tapped on 'edit' button");
            app.Tap(x => x.Marked("First").Sibling());
            app.ClearText(x => x.Marked("First").Sibling());
            app.EnterText("New Name");
            app.Screenshot("Entered 'New Name' into First Name Field");
            app.Tap(x => x.Marked("Last").Sibling());
            app.ClearText(x => x.Marked("Last").Sibling());
            app.EnterText("In List");
            app.Screenshot("Entered 'In List' into Last Name Field");
            app.Tap(x => x.Id("save"));
            app.Screenshot("Saved contact");
            app.Tap(x => x.Marked("Back"));
            app.Screenshot("Tapped on 'back' button");
            app.ScrollDownTo(x => x.Marked("In List, New Name"));
            app.Screenshot("Verified edited name appears in list");
        }

        [Test]
        public void UpdateCustomerAddress ()
        {
            app.Tap(x => x.Marked("Boone, John"));
            app.Screenshot("Selected contact with name: 'Boone, John'");
            app.WaitForElement(x => x.Class("MKNewAnnotationContainerView"));
            app.Screenshot("Verified map is set to current address");
            app.Tap(x => x.Marked("edit"));
            app.Screenshot("Tapped on 'edit' button");
            app.ScrollDownTo(x => x.Marked("Street").Sibling());
            app.Tap(x => x.Marked("Street").Sibling());
            app.ClearText(x => x.Marked("Street").Sibling());
            app.EnterText("394 Pacific Ave");
            app.Screenshot("Entered '394 Pacific Ave' into Street Field");
            app.PressEnter();
            app.ScrollDownTo("City");
            app.Tap(x => x.Marked("City").Sibling());
            app.ClearText(x => x.Marked("City").Sibling());
            app.EnterText("San Francisco");
            app.Screenshot("Entered 'San Francisco' into City Field");
            app.PressEnter();
            app.Tap(x => x.Marked("State").Sibling());
            app.ClearText(x => x.Marked("State").Sibling());
            app.EnterText("CA");
            app.Screenshot("Entered 'CA' into State Field");
            app.PressEnter();
            app.Tap(x => x.Marked("ZIP").Sibling());
            app.ClearText(x => x.Marked("ZIP").Sibling());
            app.EnterText("94111");
            app.Screenshot("Entered '94111' into ZIP Field");
            app.Tap(x => x.Id("save"));
            app.Screenshot("Saved contact");
            app.WaitForElement(x => x.Class("MKNewAnnotationContainerView"));
            app.Screenshot("Verified map is set to new address");
        }

        [Test]
        public void AddNewCustomer ()
        {
            app.Tap(x => x.Marked("add"));
            app.Screenshot("Tapped on 'add' button");
            app.Tap(x => x.Marked("First").Sibling());
            app.EnterText("NEW");
            app.Screenshot("Entered 'NEW' into First Name Field");
            app.PressEnter();
            app.Tap(x => x.Marked("Last").Sibling());
            app.EnterText("CONTACT");
            app.Screenshot("Entered 'CONTACT' into Last Name Field");
            app.PressEnter();
            app.Tap(x => x.Marked("Company").Sibling());
            app.EnterText("Xamarin");
            app.Screenshot("Entered 'Xamarin' into Company Field");
            app.PressEnter();
            app.Tap(x => x.Marked("Title").Sibling());
            app.EnterText("Test Cloud");
            app.Screenshot("Entered 'Test Cloud' into Title Field");
            app.PressEnter();
            app.Tap(x => x.Marked("Phone").Sibling());
            app.EnterText("1234567890");
            app.Screenshot("Entered '1234567890' into Phone Field");
            app.DismissKeyboard();
            app.Tap(x => x.Marked("Email").Sibling());
            app.EnterText("hello@xamarin.com");
            app.Screenshot("Entered 'hello@xamarin.com' into Email Field");
            app.PressEnter();
            app.Tap(x => x.Marked("Street").Sibling());
            app.EnterText("394 Pacific Ave");
            app.Screenshot("Entered '394 Pacific Ave' into Street Field");
            app.PressEnter();
            app.Tap(x => x.Marked("City").Sibling());
            app.EnterText("San Francisco");
            app.Screenshot("Entered 'San Francisco' into City Field");
            app.PressEnter();
            app.Tap(x => x.Marked("State").Sibling());
            app.EnterText("CA");
            app.Screenshot("Entered 'CA' into State Field");
            app.PressEnter();
            app.Tap(x => x.Marked("ZIP").Sibling());
            app.EnterText("94111");
            app.Screenshot("Entered '94111' into City Field");
            app.DismissKeyboard();
            app.Tap(x => x.Id("save"));
            app.Screenshot("Saved contact");
            app.WaitForElement("NEW CONTACT", timeout:TimeSpan.FromSeconds(10));
            app.Screenshot("Verified on NEW CONTACT's Details Page");
        }
            
        [TestCase("message")]
        [TestCase("phone")]
        [TestCase("email")]
        [TestCase("directions")]
        public void VerifyExternalLink (string link)
        {
            app.Tap(x => x.Marked("Bell, Floyd"));
            app.Screenshot("Selected contact with name: 'Bell, Floyd'");

            app.Tap(link);
            System.Threading.Thread.Sleep(5000);
            app.Screenshot(String.Format("Verify {0} opened", link));
        }
    }
}

