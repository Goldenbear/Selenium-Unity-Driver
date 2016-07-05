//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//See the NOTICE file distributed with this work for additional
//information regarding copyright ownership.
//You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace Appium.Interfaces.Generic.SearchContext
{
    /// <summary>
    /// Defines the interface through which the user finds elements by XPath.
    /// </summary>
    public interface IGenericFindsByXPath<W> where W : IWebElement
    {
        /// <summary>
        /// Finds the first element matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">The XPath query to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        W FindElementByXPath(string xpath);

        /// <summary>
        /// Finds all elements matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">The XPath query to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<W> FindElementsByXPath(string xpath);
    }
}
