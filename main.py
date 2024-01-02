import os
import subprocess

def fix_parsing(hdiffiles, deletefiles):
    for path in [hdiffiles, deletefiles]:
        with open(path, "r+") as file:
            data = file.read()
            parsed = data.replace("/", "\\").replace('{"remoteName": "', "").replace('"}', "")
            file.seek(0)
            file.write(parsed)
            file.truncate()

def apply_patch():
    with open("hdifffiles.txt", "r") as file:
        for line in file:
            filename = line.strip()
            subprocess.run([".\\lib\\hpatchz.exe", "-f", filename, filename + ".hdiff", filename])

def main():
    working_dir = os.path.dirname(os.path.abspath(__file__))

    fix_parsing("hdifffiles.txt", "deletefiles.txt")

    missing_files = []
    required_files = []

    with open("hdifffiles.txt", "r") as file:
        required_files.extend(file.read().splitlines())
    for filename in required_files:
        if not os.path.exists(os.path.join(working_dir, filename)):
            missing_files.append(filename)

    if missing_files:
        print("Missing files:")
        for filename in missing_files:
            print(f"  - {filename}")
        while True:
            choice = input("Retry patch application now? (y/n): ").lower()
            if choice in ("y", "n"):
                break
            print("Invalid input. Please enter 'y' or 'n'.")

        if choice == "n":
            print("Aborted patch application.")
            return
        
    while True:
       start = input("Start patching? (y/n): ")
       if start.lower() == "y":
           apply_patch()
           break
       elif start.lower() == "n":
           print("Aborted patch application.")
           return
       else:
           print("Invalid input. Please enter 'y' or 'n'.")

    print("Applying patch...")
    apply_patch()

if __name__ == "__main__":
    main()