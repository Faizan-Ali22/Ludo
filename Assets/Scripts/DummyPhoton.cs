using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExitGames.Client.Photon {
    public class Hashtable : Dictionary<object, object> { }
    
    public enum DebugLevel { ERROR, WARNING, INFO, ALL }
    
    public enum ConnectionProtocol { Udp, Tcp, WebSocket, WebSocketSecure }
    
    public class OperationResponse {
        public short OperationCode;
        public short ReturnCode;
        public string DebugMessage;
        public Dictionary<byte, object> Parameters;
    }
}

public class RoomOptions {
    public bool IsVisible;
    public bool IsOpen;
    public int MaxPlayers;
    public ExitGames.Client.Photon.Hashtable CustomRoomProperties;
    public string[] CustomRoomPropertiesForLobby;
}

public class ChatGui : MonoBehaviour {
    public string UserName;
    public void Connect() {}
}

public class PickupItem : MonoBehaviour {
    public void OnTriggerEnter(Collider other) {}
}

public class SupportLogging : MonoBehaviour {
    public bool LogTrafficStats;
}

public static class SupportClass {
    public static string DictionaryToString(IDictionary dict) { return ""; }
}

public class PhotonStream {
    public bool isWriting;
    public bool isReading;
    public void SendNext(object obj) {}
    public object ReceiveNext() { return null; }
    public object[] ToArray() { return null; }
}

public struct PhotonMessageInfo {
    public int timestamp;
    public PhotonPlayer sender;
    public PhotonView photonView;
}

public class PhotonView : MonoBehaviour {
    public int viewID;
    public bool isMine;
    public void RPC(string methodName, PhotonTargets target, params object[] parameters) {}
    public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters) {}
}



public class PunBehaviour : MonoBehaviour {
    public virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient) { }
    public virtual void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }
    public virtual void OnPhotonPlayerDisconnected(PhotonPlayer player) { }
    public virtual void OnDisconnectedFromPhoton() { }
    public virtual void OnPhotonRandomJoinFailed() { }
    public virtual void OnPhotonRandomJoinFailed(object[] codeAndMsg) { }
    public virtual void OnPhotonCreateRoomFailed(object[] codeAndMsg) { }
    public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg) { }
    public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer) { }
    public virtual void OnConnectedToMaster() { }
    
    public virtual void OnJoinedLobby() { }
    public virtual void OnLeftLobby() { }
    public virtual void OnJoinedRoom() { }
    public virtual void OnCreatedRoom() { }
    public virtual void OnLeftRoom() { }
    
    public virtual void OnConnectedToPhoton() { }
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause) { }
    public virtual void OnConnectionFail(DisconnectCause cause) { }
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info) { }
    public virtual void OnReceivedRoomListUpdate() { }
    public virtual void OnPhotonMaxCccuReached() { }
    public virtual void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) { }
    public virtual void OnUpdatedFriendList() { }
    public virtual void OnCustomAuthenticationFailed(string debugMessage) { }
    public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
    public virtual void OnWebRpcResponse(ExitGames.Client.Photon.OperationResponse response) { }
    public virtual void OnOwnershipRequest(object[] viewAndPlayer) { }
    public virtual void OnLobbyStatisticsUpdate() { }
    public virtual void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer) { }
    public virtual void OnOwnershipTransfered(object[] viewAndPlayers) { }
}

public class PhotonPlayer {
    public int ID;
    public string NickName = "";
    public bool IsLocal;
    public bool IsMasterClient;
    public ExitGames.Client.Photon.Hashtable CustomProperties = new ExitGames.Client.Photon.Hashtable();
    public void SetCustomProperties(ExitGames.Client.Photon.Hashtable propertiesToSet) { }
}

public class Room {
    public int PlayerCount;
    public int MaxPlayers;
    public bool IsOpen;
    public bool IsVisible;
    public string Name = "";
    public ExitGames.Client.Photon.Hashtable CustomProperties = new ExitGames.Client.Photon.Hashtable();
    public void SetCustomProperties(ExitGames.Client.Photon.Hashtable propertiesToSet) { }
}



public class PhotonHandler : MonoBehaviour {
    public static CloudRegionCode BestRegionCodeInPreferences = CloudRegionCode.eu;
}



public class RoomInfo {
    public string Name;
    public int PlayerCount;
    public int MaxPlayers;
    public bool IsOpen;
    public bool IsVisible;
    public ExitGames.Client.Photon.Hashtable CustomProperties = new ExitGames.Client.Photon.Hashtable();
}

public static class PhotonNetwork {
    public delegate void EventCallback(byte eventCode, object content, int senderId);
    public static event EventCallback OnEventCall;
    public static ServerSettings PhotonServerSettings = new ServerSettings();
    public static float BackgroundTimeout;
    public static PhotonPlayer player = new PhotonPlayer();
    public static PhotonPlayer[] otherPlayers = new PhotonPlayer[0];
    public static PhotonPlayer[] playerList = new PhotonPlayer[0];
    public static Room room = new Room();
    public static bool isMasterClient = true;
    public static AuthenticationValues AuthValues;
    public static string playerName = "";
    public static bool inRoom = false;
    public static bool connected = true;
    public static bool offlineMode = false;
    public static bool connectedAndReady = true;
    public static int countOfPlayers = 0;
    public static int countOfRooms = 0;
    public static float time = 0f;
    
    public static void LeaveRoom() { }
    public static void LeaveLobby() { }
    public static void RaiseEvent(byte eventCode, object eventContent, bool sendReliable, object options) { }
    public static void ConnectUsingSettings(string version) { }
    public static void JoinRoom(string roomName) { }
    public static void Disconnect() { }
    public static void JoinLobby() { }
    public static void JoinRandomRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties, int maxPlayers) { }
    public static void JoinRandomRoom() { }
    public static void CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby) { }
    public static RoomInfo[] GetRoomList() { return new RoomInfo[0]; }
    public static void SendOutgoingCommands() { }
    public static void LoadLevel(string name) { }
    public static void LoadLevel(int level) { }
    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data) { return null; }
    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group) { return null; }
    public static void Destroy(GameObject targetGo) { }
    public static void DestroyPlayerObjects(PhotonPlayer targetPlayer) { }
    public static bool SetMasterClient(PhotonPlayer masterClientPlayer) { return false; }
}



namespace Photon.Chat {
    public enum ChatState { Uninitialized, ConnectingToNameServer, ConnectedToNameServer, Authenticating, Authenticated, Disconnecting, Disconnected, ConnectingToFrontEnd, ConnectedToFrontEnd }
    public class AuthenticationValues {
        public CustomAuthenticationType AuthType;
        public string UserId;
        public void AddAuthParameter(string key, string value) {}
    }
    public enum CustomAuthenticationType { Custom, None }
    public interface IChatClientListener {
        void OnConnected();
        void OnSubscribed(string[] channels, bool[] results);
        void OnPrivateMessage(string sender, object message, string channelName);
        void OnChatStateChange(ChatState state);
        void OnDisconnected();
        void OnGetMessages(string channelName, string[] senders, object[] messages);
        void OnUnsubscribed(string[] channels);
        void OnStatusUpdate(string user, int status, bool gotMessage, object message);
        void OnUserSubscribed(string channel, string user);
        void OnUserUnsubscribed(string channel, string user);
    }
    public class ChatClient {
        public ChatState State;
        public ChatClient(IChatClientListener listener) { }
        public void Connect(string appId, string appVersion, global::AuthenticationValues authValues) { }
        public void Connect(string appId, string appVersion, AuthenticationValues authValues) { }
        public void Disconnect() { }
        public void Service() { }
        public void RemoveFriends(string[] friends) { }
        public void AddFriends(string[] friends) { }
        public void Subscribe(string[] channels) { }
        public void SetOnlineStatus(int status) { }
        public void SendPrivateMessage(string target, string message) { }
    }
}
