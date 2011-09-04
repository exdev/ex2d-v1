#  ======================================================================================
#  File         : Makefile
#  Author       : Wu Jie 
#  Last Change  : 08/31/2011 | 17:06:16 PM | Wednesday,August
#  Description  : 
#  ======================================================================================

# /////////////////////////////////////////////////////////////////////////////
#  general
# /////////////////////////////////////////////////////////////////////////////

# unit essential
COMPILER = /Applications/Unity/Unity.app/Contents/Frameworks/Mono/bin/gmcs
UNITY_ENGINE_DLL = /Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll
UNITY_EDITOR_DLL = /Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll
UNITY_VER = UNITY_3_4 

# Utilities.
MKDIR = mkdir -p
RM = rm -f

# Target
RUNTIME_TARGET = ../ex2d_submit/Assets/ex2D/Core/ex2D.Runtime.dll
EDITOR_TARGET = ../ex2d_submit/Assets/ex2D/Editor/ex2D.Editor.dll

# /////////////////////////////////////////////////////////////////////////////
# do build
# /////////////////////////////////////////////////////////////////////////////

.PHONY: clean rebuild

all: $(RUNTIME_TARGET) $(EDITOR_TARGET)
clean: 
	$(RM) $(RUNTIME_TARGET)
	$(RM) $(EDITOR_TARGET)
rebuild: |clean all

# /////////////////////////////////////////////////////////////////////////////
#  runtime target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Asset/
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/AnimationHelper/
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/Helper/
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/Sprite/
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Extension/
RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Helper/
RUNTIME_SOURCE = $(wildcard $(addsuffix *.cs,$(RUNTIME_SOURCE_DIRS)))

# defines
RUNTIME_DEFINE = -d:UNITY_3_4

# deubg argument
# RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -d:DEBUG -r:$(UNITY_ENGINE_DLL)
# release argument
RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -r:$(UNITY_ENGINE_DLL)

# do the build
$(RUNTIME_TARGET):
	$(COMPILER) -target:library -out:$(RUNTIME_TARGET) $(RUNTIME_ARGUMENT) $(RUNTIME_SOURCE)

# /////////////////////////////////////////////////////////////////////////////
#  editor target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/AtlasEditor/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/BitmapFontEditor/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/ComponentEditors/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/DB/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/GroupImportEditor/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/Helper/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/SpriteAnimationEditor/
EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/Utility/
EDITOR_SOURCE = $(wildcard $(addsuffix *.cs,$(EDITOR_SOURCE_DIRS)))

# defines
EDITOR_DEFINE = -d:UNITY_EDITOR -d:UNITY_3_4

# deubg argument
# EDITOR_ARGUMENT = $(EDITOR_DEFINE) -d:DEBUG -r:$(RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)
# release argument
EDITOR_ARGUMENT = $(EDITOR_DEFINE) -r:$(RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)

# do the build
$(EDITOR_TARGET): 
	$(COMPILER) -target:library -out:$(EDITOR_TARGET) $(EDITOR_ARGUMENT) $(EDITOR_SOURCE)
