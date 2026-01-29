# Release Notes - v1.0.0

## 🏰 Initial Release: Easy Dungeon Generator

**Easy Dungeon Generator** is now available! This first version includes a complete procedural dungeon generation system with full 2D/3D support, perfect for roguelikes, RPGs, and adventure games.

## ✨ **What's New in v1.0.0**

### **🎮 Core Generation System:**

1. **🔗 Connector-Based Placement**
   - Intelligent room connection using RoomConnector components
   - Automatic alignment and rotation of connected rooms
   - Visual debugging with color-coded Gizmos (green=connected, red=available)

2. **🔄 Smart Generation Algorithms**
   - Fisher-Yates shuffle for fair room selection
   - Multiple regeneration attempts with failure recovery
   - Frame-based generation for optimal performance

3. **🌍 2D/3D Project Support**
   - Automatic project type detection (2D or 3D)
   - Proper axis handling for each project type
   - Zero-configuration setup

### **⚙️ Configuration & Control:**

1. **🎯 Room Management**
   - Generate 1 to 5000 rooms in a single dungeon
   - Customizable start room and room prefab arrays
   - Adjustable minimum distance between rooms

2. **⚡ Performance Optimization**
   - Rooms per frame setting for smooth generation
   - Bounds caching for fast collision detection
   - Coroutine-based generation to prevent editor freezing

3. **🔄 Regeneration System**
   - Configurable maximum generation attempts (1-10)
   - Automatic retry on placement failure
   - Best-result preservation across attempts

### **🔧 Developer Tools:**

1. **📊 Visual Debugging**
   - Real-time connection visualization in Scene view
   - Color-coded console logging system
   - Immediate feedback on generation status

2. **🎨 Custom Inspector**
   - Branded inspector with custom banner
   - One-click generation and clearing
   - All settings exposed with tooltips

3. **📝 Logging System**
   - Four message types: Normal, Warning, Error, Successful
   - Color-coded Unity console output
   - Easy filtering with [EasyDungeonGenerator] prefix

### **🧩 Utility Components:**

1. **RoomConnector Component**
   - Simple MonoBehaviour for connection points
   - Connection state management
   - Editor visualization with Gizmos

2. **ArrayUtility Class**
   - Fisher-Yates shuffle implementation
   - Generic list shuffling for any type
   - In-place operation for performance

## 🔧 **Technical Implementation**

- **Runtime & Editor**: Full support for both modes
- **Zero Dependencies**: Uses only Unity's built-in systems
- **Namespace Organization**: Clean separation with `BreadThief.EasyDungeonGenerator`
- **Assembly Definition**: Proper asmdef setup for UPM compatibility
- **Compatibility**: Unity 2021.3+ LTS versions

## 📦 **Included Examples**

### **2D Dungeon Example**
- Sprite-based room prefabs
- 2D-optimized connector placement
- Sample scene with complete setup

### **3D Dungeon Example**
- Mesh-based room prefabs
- 3D connector rotation
- Sample scene with camera setup

## 🎯 **Use Cases**

- **Roguelike Games**: Generate unique dungeons each run
- **RPG Adventures**: Create sprawling underground complexes
- **Procedural Content**: Generate levels for endless gameplay
- **Prototyping**: Quickly test level layouts and room connections
- **Educational**: Learn about procedural generation techniques

## ⚙️ **Quick Start**

1. **Add RoomConnector components** to your room prefabs
2. **Assign prefabs** to the EasyDungeonGenerator component
3. **Click "Generate Dungeon"** - that's it!

## 📋 **Menu Commands**

- **Generate Dungeon**: Start generation (right-click on component)
- **Clear Dungeon**: Remove all generated rooms

## ⚠️ **Initial Release Notes**

This is the first stable release. The system has been tested with various room configurations and project types. All core features are production-ready.

**Known Limitations:**
- Very complex room shapes may require tuning of Min Room Distance
- Extremely large dungeons (1000+ rooms) may require performance optimization
- Requires at least one connector per room prefab

## 📦 **Installation Options**

**Via Unity Package Manager:**
```
https://github.com/bread-thief/easy-dungeon-generator.git
```

**Manual Installation:**
Copy `Plugins/EasyDungeonGenerator` folder to your `Assets/` directory

**Asset Store:**
Available soon on the Unity Asset Store!

## 👍 **Result**
A robust, easy-to-use dungeon generation system that "just works" out of the box. Perfect for developers who want procedural content without the complexity of building their own generation system.

---

**Happy dungeon crawling! 🏹🗡️🐉**