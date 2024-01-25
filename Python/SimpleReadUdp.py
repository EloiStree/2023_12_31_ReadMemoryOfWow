import socket

def receive_udp_messages():
    # Set the listening address and port
    listen_ip = "0.0.0.0"  # Listen on all available interfaces
    listen_port = 12345

    # Create a UDP socket
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as udp_socket:
        # Bind the socket to the specified address and port
        udp_socket.bind((listen_ip, listen_port))

        print(f"Listening for UDP messages on {listen_ip}:{listen_port}")

        while True:
            try:
                # Receive data from the socket
                data, address = udp_socket.recvfrom(1500)  # Adjust buffer size as needed

                # Decode and print the received message
                message = data.decode("utf-8")
                print(f"Received message from {address[0]}:{address[1]}: {message}")
            except:
                 print(f"Something happened wrong")
            
                

if __name__ == "__main__":
    receive_udp_messages()
