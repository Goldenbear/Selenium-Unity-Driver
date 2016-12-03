#!/bin/bash

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
brew install --HEAD ideviceinstaller
brew install carthage
sudo npm install -g node-inspector
cd ../
