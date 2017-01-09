#!/bin/bash

#XCUITest setup
printf "\e[1;31m*** Setting up Appium XCUITest support...\n\e[0;30m"
sudo npm install -g ios-deploy
sudo npm install -g deviceconsole
brew install ideviceinstaller
brew install carthage
npm install xcpretty
brew install libimobiledevice --HEAD
cd Appium/node_modules/appium-xcuitest-driver/WebDriverAgent
mkdir -p Resources/WebDriverAgent.bundle
sh ./Scripts/bootstrap.sh -d
echo "DEVELOPMENT_TEAM = P237U2XMJR\nCODE_SIGN_IDENTITY = iPhone Developer" > WebDriver.xcconfig
cd ../../../../
printf "\e[1;31m*** Appium XCUITest setup complete. ***\n\e[0;30m"
