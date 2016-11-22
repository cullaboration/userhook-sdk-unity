#!/bin/bash

# remove app specific configuration files before creating unitypackage

rm -f Assets/Resources/userhook.txt
rm -f Assets/Plugins/Android/google-services/res/values/values.xml
rm -f Assets/Plugins/Android/userhook-*/AndroidManifest.xml
rm -f Assets/Plugins/Android/google-services.json