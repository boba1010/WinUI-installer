# WinUI Installer

A lightweight, generic installer built using WinUI 3 for modern, native Windows 11 Fluent UI layouts. It includes a custom-engineered, UAC-safe folder selection grid that runs flawlessly under elevated administrative privileges.

## UI Previews
<p align="center">
  <img src="https://github.com/user-attachments/assets/7b118946-e82f-4279-a64a-8b0f8a10993e" width="45%" alt="Welcome Setup Screen" />
  <img src="https://github.com/user-attachments/assets/7a4afcfe-445a-4a3d-aeb3-e0d93a4e4a38" width="45%" alt="Installation Location Screen" />
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/aa9dfc0f-295f-4aa6-b53c-d7e26f19b13b" width="45%" alt="Custom UAC-Safe Folder Dialog" />
  <img src="https://github.com/user-attachments/assets/e080b88e-f0f9-4091-af6a-7233197a4581" width="45%" alt="Installation Complete Screen" />
</p>

---

## Features
* **Modern Fluent UI:** Native Windows 11 theme matching with smooth layouts and hover animations.
* **UAC-Safe Navigation:** Custom File Explorer replacement interface built with ContentDialog + GridView to fix elevated `FolderPicker` crashes without unsafe low-level API pointer hooks.
* **Zero Dependencies:** Completely managed, highly stable standalone deployment bundle.

---

## How to Use

### 1. Configure the Application Meta
Open `InstallerPrepareAPI.cs` in your local development environment and input your specific project settings:
* Change the default application name template.
* Provide your exact target `.exe` binary string.

### 2. Bundle Your Application
Locate the `Embedded` folder inside the project layout:
1. Compress your application build files into a standard archive named exactly `app.zip`.
2. **Important:** Your files must sit directly at the root level inside the `.zip` archive (do not compress them inside an isolated top-level directory).
3. Replace the placeholder package inside the `Embedded` folder with your new `app.zip` deployment file.

### 3. Build and Distribute
Open the solution file inside Visual Studio and run your build command. Your native installer executable is instantly optimized and ready to ship.

---

## License
This project is licensed under the MIT License - see the LICENSE.txt file for details.
