@echo off
git rm --cached Assets/Plugins/FMOD/Resources/FMODStudioCache.asset
git rm --cached Assets/Plugins/FMOD/Resources/FMODStudioCache.asset.meta
git rm fmod_editor.log

rm Assets/Plugins/FMOD/Resources/FMODStudioCache.asset
rm Assets/Plugins/FMOD/Resources/FMODStudioCache.asset.meta
rm fmod_editor.log

