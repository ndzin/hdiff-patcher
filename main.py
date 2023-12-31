import os
import subprocess

def process_files(file1_path, file2_path):

    with open(file1_path, "r") as file1, open(file2_path, "r") as file2:
        contents1 = file1.read()
        contents2 = file2.read()
        
    contents1 = contents1.replace("/", "\\", )
    contents1 = contents1.replace('{"remoteName": "', "")
    contents1 = contents1.replace('"}', "")

    contents2 = contents2.replace("/", "\\")
    contents2 = contents2.replace('{"remoteName": "', "")
    contents2 = contents2.replace('"}', "")

    with open(file1_path, "w") as file1, open(file2_path, "w") as file2:
        file1.write(contents1)
        file2.write(contents2)

def apply_patch():
    with open("hdifffiles.txt", "r") as file:
        for line in file:
            filename = line.strip()
            subprocess.run([".\\lib\\hpatchz.exe", "-f", filename, filename + ".hdiff", filename])

def main():
    working_dir = os.path.dirname(os.path.abspath(__file__))

    process_files("hdifffiles.txt", "deletefiles.txt")

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