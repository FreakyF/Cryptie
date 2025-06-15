#!/bin/sh
git config core.hooksPath .githooks
chmod +x .githooks/*
git rm -r --cached ./.coverage/