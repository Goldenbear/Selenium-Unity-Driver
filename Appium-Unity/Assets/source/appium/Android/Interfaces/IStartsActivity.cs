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
namespace OpenQA.Selenium.Appium.Android
{
    public interface IStartsActivity
    {
        void StartActivity(string appPackage, string appActivity, string appWaitPackage = "", string appWaitActivity = "", bool stopApp = true);

        void StartActivityWithIntent(string appPackage, string appActivity, string intentAction, string appWaitPackage = "", string appWaitActivity = "",
             string intentCategory = "", string intentFlags = "", string intentOptionalArgs = "", bool stopApp = true);

        string CurrentActivity
        {
            get;
        }
    }
}
