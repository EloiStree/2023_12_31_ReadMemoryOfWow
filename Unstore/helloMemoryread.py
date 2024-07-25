import ctypes
import psutil
import time
import pyautogui

# Define the process ID and memory address
process_id = 0x6BFC
memory_address_player_position_x = 0x2A1AA4BE0C0
memory_address_player_position_y = 0x2A1AA4BE0C0
memory_address_player_position_z = 0x2A1AA4BE0C0
memory_address_player_position_xp = 0x2A1AA4BE0C0

# Open the process with necessary permissions
PROCESS_ALL_ACCESS = 0x1F0FFF
process_handle = ctypes.windll.kernel32.OpenProcess(PROCESS_ALL_ACCESS, False, process_id)

# Define a function to read a float value from a specific memory address
def read_float_from_memory(process_handle, address):
    buffer = ctypes.c_float()
    bytesRead = ctypes.c_size_t()
    if ctypes.windll.kernel32.ReadProcessMemory(process_handle, ctypes.c_void_p(address), ctypes.byref(buffer), ctypes.sizeof(buffer), ctypes.byref(bytesRead)):
        return buffer.value
    else:
        return None

try:
    while True:
        value = read_float_from_memory(process_handle, memory_address)
        if value is not None:
            print(f"Float value at address {hex(memory_address)}: {value}")
            pyautogui.press('w')
            time.sleep(1)
            pyautogui.release('w')
                
                
        else:
            print("Failed to read memory")
        time.sleep(1)
finally:
    ctypes.windll.kernel32.CloseHandle(process_handle)


