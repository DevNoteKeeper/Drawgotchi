# Thanks random guy on linkedin: https://www.linkedin.com/pulse/visualizing-wacom-stylus-data-real-time-python-nikos-mouzakitis-outsf/
import asyncio
import sys
import threading
from evdev import InputDevice, list_devices, ecodes
import matplotlib.pyplot as plt
import matplotlib.animation as animation

max_points = 200
data = {"ABS_X": [], "ABS_Y": [], "ABS_PRESSURE": [], "ABS_DISTANCE": []}

def find_wacom_device_name():
    for path in list_devices():
        dev = InputDevice(path)
        print(dev.path, dev.name)
        if 'Wacom' in dev.name or 'CTL-672' in dev.name or 'One by Wacom' in dev.name:
            return dev.path, dev.name
    return None, None

async def read_events(path):
    dev = InputDevice(path)
    print(f"Listening on {dev.path} ({dev.name}) - ctrl-c to stop it")
    async for ev in dev.async_read_loop():
        if ev.type == ecodes.EV_ABS:
            absname = ecodes.ABS.get(ev.code, str(ev.code))
            ## debug to find the ylims
            #print(f"[{ev.timestamp()}] ABS {absname} value={ev.value}")
            if absname in data:
                data[absname].append(ev.value)
                if len(data[absname]) > max_points:
                    data[absname].pop(0)

# ----- Matplotlib plotting -----
fig, axs = plt.subplots(4, 1, figsize=(8, 8))
lines = {}
keys = ["ABS_X", "ABS_Y", "ABS_PRESSURE", "ABS_DISTANCE"]
y_limits = {
    "ABS_X": (0, 22000),
    "ABS_Y": (0, 13000),
    "ABS_PRESSURE": (0, 2000),
    "ABS_DISTANCE": (0, 64)
}

##set our custom y lims accordin to the values got from the device
for ax, key in zip(axs, keys):
    ax.set_title(key)
    ax.set_xlim(0, max_points)       # horizontal axis: last N points
    ax.set_ylim(*y_limits[key])      # custom Y-axis limits
    line, = ax.plot([], [], lw=2)
    lines[key] = line

def update(frame):
    for key in keys:
        y = data[key]
        x = list(range(len(y)))
        lines[key].set_data(x, y)
    return lines.values()

ani = animation.FuncAnimation(fig, update, interval=50, blit=True)

# ----- Main -----
path, name = find_wacom_device_name()
if not path:
    print("Wacom dev not found")
    sys.exit()

# Create a background thread to run the asyncio loop
loop = asyncio.new_event_loop()
def start_loop():
    asyncio.set_event_loop(loop)
    loop.run_until_complete(read_events(path))

threading.Thread(target=start_loop, daemon=True).start()

plt.tight_layout()
plt.show() 
