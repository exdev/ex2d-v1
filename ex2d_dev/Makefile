#  ======================================================================================
#  File         : Makefile
#  Author       : Wu Jie 
#  Last Change  : 08/31/2011 | 17:06:16 PM | Wednesday,August
#  Description  : 
#  ======================================================================================

# /////////////////////////////////////////////////////////////////////////////
#  general
# /////////////////////////////////////////////////////////////////////////////

# settings
UNITY_PATH = /Applications/Unity/Unity.app/Contents/Frameworks

# unit essential
COMPILER = $(UNITY_PATH)/Mono/bin/gmcs
UNITY_ENGINE_DLL = $(UNITY_PATH)/Managed/UnityEngine.dll
UNITY_EDITOR_DLL = $(UNITY_PATH)/Managed/UnityEditor.dll
UNITY_VER = UNITY_3_5 

# Utilities.
MKDIR = mkdir -p
RM = rm -f

# ex2D Target
EX2D_TARGET_DIR = ../ex2d_submit/Assets/ex2D
EX2D_RUNTIME_TARGET = $(EX2D_TARGET_DIR)/Core/ex2D.Runtime.dll
EX2D_EDITOR_TARGET = $(EX2D_TARGET_DIR)/Editor/ex2D.Editor.dll

#  # ex2D TileMap Target
#  EX2D_TILEMAP_TARGET_DIR = ../ex2d_tilemap_submit/Assets/ex2D_TileMap
#  EX2D_TILEMAP_RUNTIME_TARGET = $(EX2D_TILEMAP_TARGET_DIR)/Core/ex2D.TileMap.Runtime.dll
#  EX2D_TILEMAP_EDITOR_TARGET = $(EX2D_TILEMAP_TARGET_DIR)/Editor/ex2D.TileMap.Editor.dll

# ex2D GUI Target
EX2D_GUI_TARGET_DIR = ../ex2d_gui_submit/Assets/ex2D_GUI
EX2D_GUI_RUNTIME_TARGET = $(EX2D_GUI_TARGET_DIR)/Core/ex2D.Runtime.dll
EX2D_GUI_EDITOR_TARGET = $(EX2D_GUI_TARGET_DIR)/Editor/ex2D.Editor.dll

# /////////////////////////////////////////////////////////////////////////////
# do build
# /////////////////////////////////////////////////////////////////////////////

.PHONY: clean rebuild

ex2d: $(EX2D_RUNTIME_TARGET) $(EX2D_EDITOR_TARGET)
clean_ex2d: 
	$(RM) $(EX2D_RUNTIME_TARGET)
	$(RM) $(EX2D_EDITOR_TARGET)
rebuild_ex2d: |clean_ex2d ex2d 

#  tilemap: $(EX2D_TILEMAP_RUNTIME_TARGET) $(EX2D_TILEMAP_EDITOR_TARGET)
#  clean_tilemap: 
#      $(RM) $(EX2D_TILEMAP_RUNTIME_TARGET) 
#      $(RM) $(EX2D_TILEMAP_EDITOR_TARGET)
#  rebuild_tilemap: |clean_tilemap tilemap 

gui: $(EX2D_GUI_RUNTIME_TARGET) $(EX2D_GUI_EDITOR_TARGET) 
clean_gui: 
	$(RM) $(EX2D_GUI_RUNTIME_TARGET) 
	$(RM) $(EX2D_GUI_EDITOR_TARGET) 
rebuild_gui: |clean_gui gui 

clean_all: |clean_ex2d
all: |ex2d
rebuild_all: |clean_all all

# defines
RUNTIME_DEFINE = -d:$(UNITY_VER) -d:UNITY_IPHONE
EDITOR_DEFINE = -d:$(UNITY_VER) -d:UNITY_EDITOR 

# /////////////////////////////////////////////////////////////////////////////
# ex2D runtime target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Asset/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/AnimationHelper/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/Camera/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/Helper/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/Manager/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Component/Sprite/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Deprecated/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Extension/
EX2D_RUNTIME_SOURCE_DIRS += Assets/ex2D/Core/Helper/
EX2D_RUNTIME_SOURCE = $(wildcard $(addsuffix *.cs,$(EX2D_RUNTIME_SOURCE_DIRS)))

# deubg argument
# EX2D_RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -d:DEBUG -r:$(UNITY_ENGINE_DLL)
# release argument
EX2D_RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -r:$(UNITY_ENGINE_DLL)

# do the build
$(EX2D_RUNTIME_TARGET):
	@echo "========================================================" 
	@echo Building ex2D.Runtime.dll...
	@echo "========================================================" 
	rm -rf $(EX2D_TARGET_DIR)/Core/Shaders/
	cp -r Assets/ex2D/Core/Shaders/ $(EX2D_TARGET_DIR)/Core/Shaders
	$(COMPILER) -target:library -out:$(EX2D_RUNTIME_TARGET) $(EX2D_RUNTIME_ARGUMENT) $(EX2D_RUNTIME_SOURCE)
	@echo done!
	@echo 

# /////////////////////////////////////////////////////////////////////////////
# ex2D editor target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/About/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/AtlasEditor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/BitmapFontEditor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/ComponentEditors/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/DB/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/GUIBorderEditor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/GroupImportEditor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/Helper/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/LayerEditor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/Misc/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/SpriteAnimationEditor/
EX2D_EDITOR_SOURCE_DIRS += Assets/ex2D/Editor/Utility/
EX2D_EDITOR_SOURCE = $(wildcard $(addsuffix *.cs,$(EX2D_EDITOR_SOURCE_DIRS)))

# resources
# EX2D_RESOURCES += -resource:Assets/ex2D/Editor/Resource/pixel.png
# EX2D_RESOURCES += -resource:Assets/ex2D/Editor/Resource/checkerboard_64x64.png
# EX2D_RESOURCES += -resource:Assets/ex2D/Editor/Resource/border.png
# EX2D_RESOURCES += -resource:Assets/ex2D/Editor/Resource/rect_select.png

# deubg argument
# EX2D_EDITOR_ARGUMENT = $(EDITOR_DEFINE) -d:DEBUG -r:$(EX2D_RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)
# release argument
EX2D_EDITOR_ARGUMENT = $(EDITOR_DEFINE) $(EX2D_RESOURCES) -r:$(EX2D_RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)

# do the build
$(EX2D_EDITOR_TARGET): 
	@echo "========================================================" 
	@echo building ex2D.Editor.dll...
	@echo "========================================================" 
	rm -rf $(EX2D_TARGET_DIR)/Editor/Resource/
	cp -r Assets/ex2D/Editor/Resource/ $(EX2D_TARGET_DIR)/Editor/Resource
	$(COMPILER) -target:library -out:$(EX2D_EDITOR_TARGET) $(EX2D_EDITOR_ARGUMENT) $(EX2D_EDITOR_SOURCE)
	@echo done!
	@echo

# /////////////////////////////////////////////////////////////////////////////
# ex2D TileMap runtime target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EX2D_TILEMAP_RUNTIME_SOURCE_DIRS += Assets/ex2D_TileMap/Core/
EX2D_TILEMAP_RUNTIME_SOURCE = $(wildcard $(addsuffix *.cs,$(EX2D_TILEMAP_RUNTIME_SOURCE_DIRS)))

# deubg argument
# EX2D_TILEMAP_RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -d:DEBUG -r:$(UNITY_ENGINE_DLL),$(EX2D_RUNTIME_TARGET)
# release argument
EX2D_TILEMAP_RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -r:$(UNITY_ENGINE_DLL),$(EX2D_RUNTIME_TARGET)

# do the build
$(EX2D_TILEMAP_RUNTIME_TARGET):
	@echo "========================================================" 
	@echo building ex2D.TileMap.Runtime.dll...
	@echo "========================================================" 
	$(COMPILER) -target:library -out:$(EX2D_TILEMAP_RUNTIME_TARGET) $(EX2D_TILEMAP_RUNTIME_ARGUMENT) $(EX2D_TILEMAP_RUNTIME_SOURCE)
	@echo done!
	@echo

# /////////////////////////////////////////////////////////////////////////////
# ex2D TileMap editor target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EX2D_TILEMAP_EDITOR_SOURCE_DIRS += Assets/ex2D_TileMap/Editor/ComponentEditors/
EX2D_TILEMAP_EDITOR_SOURCE_DIRS += Assets/ex2D_TileMap/Editor/TileMapEditor/
EX2D_TILEMAP_EDITOR_SOURCE_DIRS += Assets/ex2D_TileMap/Editor/TileSheetEditor/
EX2D_TILEMAP_EDITOR_SOURCE_DIRS += Assets/ex2D_TileMap/Editor/Utility/
EX2D_TILEMAP_EDITOR_SOURCE = $(wildcard $(addsuffix *.cs,$(EX2D_TILEMAP_EDITOR_SOURCE_DIRS)))

# deubg argument
# EX2D_TILEMAP_EDITOR_ARGUMENT = $(EDITOR_DEFINE) -d:DEBUG -r:$(EX2D_RUNTIME_TARGET),$(EX2D_EDITOR_TARGET),$(EX2D_TILEMAP_RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)
# release argument
EX2D_TILEMAP_EDITOR_ARGUMENT = $(EDITOR_DEFINE) -r:$(EX2D_RUNTIME_TARGET),$(EX2D_EDITOR_TARGET),$(EX2D_TILEMAP_RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)

# do the build
$(EX2D_TILEMAP_EDITOR_TARGET): 
	@echo "========================================================" 
	@echo building ex2D.TileMap.Editor.dll...
	@echo "========================================================" 
	$(COMPILER) -target:library -out:$(EX2D_TILEMAP_EDITOR_TARGET) $(EX2D_TILEMAP_EDITOR_ARGUMENT) $(EX2D_TILEMAP_EDITOR_SOURCE)
	@echo done!
	@echo

# /////////////////////////////////////////////////////////////////////////////
# ex2D GUI runtime target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EX2D_GUI_RUNTIME_SOURCE_DIRS += Assets/ex2D_GUI/Core/Asset/
EX2D_GUI_RUNTIME_SOURCE_DIRS += Assets/ex2D_GUI/Core/Component/
EX2D_GUI_RUNTIME_SOURCE_DIRS += Assets/ex2D_GUI/Core/Effect/
EX2D_GUI_RUNTIME_SOURCE_DIRS += Assets/ex2D_GUI/Core/Effect/EffectFrom/
EX2D_GUI_RUNTIME_SOURCE_DIRS += Assets/ex2D_GUI/Core/Effect/EffectTo/
EX2D_GUI_RUNTIME_SOURCE_DIRS += Assets/ex2D_GUI/Core/Helper/
EX2D_GUI_RUNTIME_SOURCE = $(wildcard $(addsuffix *.cs,$(EX2D_GUI_RUNTIME_SOURCE_DIRS)))

# deubg argument
# EX2D_GUI_RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -d:DEBUG -r:$(UNITY_ENGINE_DLL)
# release argument
EX2D_GUI_RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -r:$(UNITY_ENGINE_DLL)

# do the build
$(EX2D_GUI_RUNTIME_TARGET):
	@echo "========================================================" 
	@echo building ex2D.GUI.Runtime.dll...
	@echo "========================================================" 
	$(COMPILER) -target:library -out:$(EX2D_GUI_RUNTIME_TARGET) $(EX2D_GUI_RUNTIME_ARGUMENT) $(EX2D_RUNTIME_SOURCE) $(EX2D_GUI_RUNTIME_SOURCE)
	@echo done!
	@echo

# /////////////////////////////////////////////////////////////////////////////
# ex2D GUI editor target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
EX2D_GUI_EDITOR_SOURCE_DIRS += Assets/ex2D_GUI/Editor/
EX2D_GUI_EDITOR_SOURCE_DIRS += Assets/ex2D_GUI/Editor/ComponentEditor/
EX2D_GUI_EDITOR_SOURCE_DIRS += Assets/ex2D_GUI/Editor/TimebasedCurve/
EX2D_GUI_EDITOR_SOURCE_DIRS += Assets/ex2D_GUI/Editor/Utility/
EX2D_GUI_EDITOR_SOURCE = $(wildcard $(addsuffix *.cs,$(EX2D_GUI_EDITOR_SOURCE_DIRS)))

# deubg argument
# EX2D_GUI_EDITOR_ARGUMENT = $(EDITOR_DEFINE) -d:DEBUG -r:$(EX2D_GUI_RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)
# release argument
EX2D_GUI_EDITOR_ARGUMENT = $(EDITOR_DEFINE) $(EX2D_RESOURCES) -r:$(EX2D_GUI_RUNTIME_TARGET),$(UNITY_ENGINE_DLL),$(UNITY_EDITOR_DLL)

# do the build
$(EX2D_GUI_EDITOR_TARGET): 
	@echo "========================================================" 
	@echo building ex2D.GUI.Editor.dll...
	@echo "========================================================" 
	$(COMPILER) -target:library -out:$(EX2D_GUI_EDITOR_TARGET) $(EX2D_GUI_EDITOR_ARGUMENT) $(EX2D_EDITOR_SOURCE) $(EX2D_GUI_EDITOR_SOURCE)
	@echo done!
	@echo
