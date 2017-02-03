#!/bin/bash

#
#Appium installation script
#

printf "\e[1;31m*** Installing Appium support components...\n\e[0;30m"

#Install global components that require admin password first so this isnt requested half way through.
sudo npm install -g node-inspector
sudo npm install -g ios-deploy --unsafe-perm=true --allow-root
sudo npm install -g deviceconsole --unsafe-perm=true --allow-root

#Install brew if not already
which -s brew
if [[ $? != 0 ]] ; then
    # Install Homebrew
    ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
else
    brew update
fi
brew install rbenv ruby-build
echo 'if which rbenv > /dev/null; then eval "$(rbenv init -)"; fi' >> ~/.bash_profile
source ~/.bash_profile
rbenv install 2.3.1
rbenv global 2.3.1
ruby -v
cd ../

#XCUITest support installation
brew install ideviceinstaller
brew install carthage
npm install xcpretty
#Uncomment this if getting 'Unable to start WebDriverAgent:' errors:
#brew unlink libimobiledevice
brew install libimobiledevice --HEAD

cd Appium/node_modules/appium-xcuitest-driver/WebDriverAgent
mkdir -p Resources/WebDriverAgent.bundle
sh ./Scripts/bootstrap.sh -d
echo "DEVELOPMENT_TEAM = P237U2XMJR\nCODE_SIGN_IDENTITY = iPhone Developer" > WebDriver.xcconfig
cd ../../../../

printf "\e[1;31m*** Appium support installation complete. ***\n\e[0;30m"
