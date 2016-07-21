using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public interface IHCPElement
{
    //
    // Summary:
    //     Gets the coordinates identifying the location of this element using various frames
    //     of reference.
    //public ICoordinates Coordinates { get; }
    //
    // Summary:
    //     Gets a value indicating whether or not this element is displayed.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    //
    // Remarks:
    //     The OpenQA.Selenium.Remote.RemoteWebElement.Displayed property avoids the problem
    //     of having to parse an element's "style" attribute to determine visibility of
    //     an element.
    bool Displayed { get; }
    //
    // Summary:
    //     Gets a value indicating whether or not this element is enabled.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    //
    // Remarks:
    //     The OpenQA.Selenium.Remote.RemoteWebElement.Enabled property will generally return
    //     true for everything except explicitly disabled input elements.
    bool Enabled { get; }
    //
    // Summary:
    //     Gets a System.Drawing.Point object containing the coordinates of the upper-left
    //     corner of this element relative to the upper-left corner of the page.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    Vector2 Location { get; }
    //
    // Summary:
    //     Gets the point where the element would be when scrolled into view.
    Vector2 LocationOnScreenOnceScrolledIntoView { get; }
    //
    // Summary:
    //     Gets a value indicating whether or not this element is selected.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    //
    // Remarks:
    //     This operation only applies to input elements such as checkboxes, options in
    //     a select element and radio buttons.
    bool Selected { get; }
    //
    // Summary:
    //     Gets a OpenQA.Selenium.Remote.RemoteWebElement.Size object containing the height
    //     and width of this element.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    Vector2 Size { get; }
    //
    // Summary:
    //     Gets the tag name of this element.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    //
    // Remarks:
    //     The OpenQA.Selenium.Remote.RemoteWebElement.TagName property returns the tag
    //     name of the element, not the value of the name attribute. For example, it will
    //     return "input" for an element specified by the HTML markup <input name="foo"
    //     />.
    string TagName { get; }
    //
    // Summary:
    //     Gets the innerText of this element, without any leading or trailing whitespace,
    //     and with other whitespace collapsed.
    //
    // Exceptions:
    //   T:OpenQA.Selenium.StaleElementReferenceException:
    //     Thrown when the target element is no longer valid in the document DOM.
    string Text { get; }
       
    //
    // Summary:
    //     Gets the ID of the element
    //
    // Remarks:
    //     This property is internal to the WebDriver instance, and is not intended to be
    //     used in your code. The element's ID has no meaning outside of internal WebDriver
    //     usage, so it would be improper to scope it as public. However, both subclasses
    //     of OpenQA.Selenium.Remote.RemoteWebElement and the parent driver hosting the
    //     element have a need to access the internal element ID. Therefore, we have two
    //     properties returning the same value, one scoped as internal, the other as protected.
    string Id { get; }
}
