PROJECT_NAME   	:= MobileSentimentDetector
NETCORE_VERSION := net9.0
PROJECT_BIN   	:= $(PROJECT_NAME)/bin/Debug/$(NETCORE_VERSION)
ARCH 						:= linux-musl-arm64
BUILD_FOLDER		:= $(PROJECT_BIN)/$(ARCH)
PUBLISH_FOLDER	:= publish
APPIMAGE_FOLDER	:= AppImage
ARCHIVE_NAME		:= $(PROJECT_NAME)-$(ARCH).tar.gz
TARGET_FOLDER		:= /usr/local/bin/$(PROJECT_NAME)
# SSH_DESTINATION	:= kde@plasma-mobile
# SSH_DESTINATION	:= manjaro@manjaro-arm
# SSH_DESTINATION	:= user@192.168.1.90
SSH_DESTINATION	:= user@pine64-pinephone
AVALONIA_DESIGNER := C://Users/nguel/.nuget/packages/avalonia/11.2.6/tools/netstandard2.0/designer/Avalonia.Designer.HostApp.dll
PROJECT_PATH := $(realpath $(PROJECT_NAME))
TRANSPORT_PATH := $(subst \,/,$(PROJECT_PATH))

clean-publish-local: cleanup restore build publish-local

run-clean: cleanup restore build run

all : cleanup restore build

cleanup:
	dotnet clean $(PROJECT_NAME)
	rm -rf $(PUBLISH_FOLDER)

restore:
	dotnet restore $(PROJECT_NAME)

build:
	dotnet build $(PROJECT_NAME)

run:
	dotnet run --project $(PROJECT_NAME)

publish:
	dotnet publish -r $(ARCH) $(PROJECT_NAME) -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -o $(PUBLISH_FOLDER) --framework $(NETCORE_VERSION) 

appimage-folder:
	rm -f $(APPIMAGE_FOLDER) && mkdir $(APPIMAGE_FOLDER)
	# Create folder structure here
	cd $(PUBLISH_FOLDER) && rm -f ../$(ARCHIVE_NAME) | tar -czvf ../$(ARCHIVE_NAME) ./*

archive:
	cd $(PUBLISH_FOLDER) && rm -f ../$(ARCHIVE_NAME) | tar -czvf ../$(ARCHIVE_NAME) ./*

install-local:
	ssh -t $(SSH_DESTINATION) '(rm -rf $(TARGET_FOLDER) 2>/dev/null || true) && sudo mkdir $(TARGET_FOLDER) && cd $(TARGET_FOLDER) && sudo mv ~/$(ARCHIVE_NAME) ../$(ARCHIVE_NAME) && sudo tar -xzvf ../$(ARCHIVE_NAME) && sudo rm -rf ../$(ARCHIVE_NAME) && sudo chmod +x $(PROJECT_NAME) && sudo mv $(PROJECT_NAME).desktop /usr/share/applications/'

publish-local: publish archive install-local

designer: build designer-run

designer-run:
	dotnet exec --runtimeconfig $(PROJECT_BIN)/$(PROJECT_NAME).runtimeconfig.json \
 		--depsfile  $(PROJECT_BIN)/$(PROJECT_NAME).deps.json $(AVALONIA_DESIGNER) \
 		--transport file:///$(TRANSPORT_PATH)/$(view) \
		--method html --html-url http://127.0.0.1:6001 \
 		$(PROJECT_BIN)/$(PROJECT_NAME).dll

design-debug:
	make view=MainWindow.axaml designer-run