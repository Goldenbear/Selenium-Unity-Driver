﻿//Licensed under the Apache License, Version 2.0 (the "License");
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
using Appium.Interfaces.Generic.SearchContext;

namespace OpenQA.Selenium.Appium.Interfaces
{
    /// <summary>
    /// This interface extends IWebElement and defines specific behavior
    /// for mobile.
    /// </summary>
    public interface IMobileElement<W> : IFindByAccessibilityId<W>, IGenericSearchContext<W>,
        IGenericFindsByClassName<W>,
        IGenericFindsById<W>, IGenericFindsByCssSelector<W>, IGenericFindsByLinkText<W>,
        IGenericFindsByName<W>,
        IGenericFindsByPartialLinkText<W>, IGenericFindsByTagName<W>, IGenericFindsByXPath<W>, IWebElement
        where W : IWebElement
    {

        /// <summary>
        /// Convenience method for pinching the given element.
        /// "pinching" refers to the action of two appendages pressing the screen and sliding towards each other.
        /// NOTE:
        /// This convenience method places the initial touches around the element, if this would happen to place one of them
        /// off the screen, appium with return an outOfBounds error. In this case, revert to using the IMultiAction api
        /// instead of this method.
        /// </summary>
        void Pinch();

        /// <summary>
        /// Convenience method for tapping the center of the given element
        /// </summary>
        /// <param name="fingers"> number of fingers/appendages to tap with </param>
        /// <param name="duration">how long between pressing down, and lifting fingers/appendages</param>
        void Tap(int fingers, int duration);

        /// <summary>
        /// Convenience method for "zooming in" on the given element.
        /// "zooming in" refers to the action of two appendages pressing the screen and sliding away from each other.
        /// NOTE:
        /// This convenience method slides touches away from the element, if this would happen to place one of them
        /// off the screen, appium will return an outOfBounds error. In this case, revert to using the IMultiAction api
        /// instead of this method.
        /// </summary>
        void Zoom();
    }
}
