#!/bin/bash

git pull --rebase
git submodule foreach git pull --rebase
