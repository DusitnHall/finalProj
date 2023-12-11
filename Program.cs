//Finding the Paths in the directory and recieving user input
static string FindPath() {
    Console.Clear();

    string directLoc = @"D:\ToBeEncrypted";

    // files list from the root directory and prints it
    string[] files = Directory.GetFiles(directLoc);

    int i = 0;
    foreach (string paths in files) {
        i ++;
        Console.WriteLine($"{i}. {paths}");
    }

    Console.Write("Select your file: ");

    return @"" + files[Convert.ToInt32(Console.ReadLine()) - 1];
}

//Return an int wrapped between the upper and lower bounds
static int Wrap(int num, int lower, int upper) {

    while(num > upper || num < lower) {
        if (num > upper) 
            num -= upper - lower + 1;
        else if (num < lower)
            num += upper - lower + 1;
    }

    return num;
}

//Encrypting the data in the file
static byte[] Encrypt(string[] hex) {

    List<int> hexList = new List<int>();

    for (int i = 0; i < hex.Length; i++) {
        hexList.Add(Convert.ToInt32(hex[i], 16));
    }

    foreach (int item in hexList) {
        Console.Write(item + " ");
    }

    Console.Write("Give your secret code word: ");
    string? code = Console.ReadLine();

    if (code == null) {
        Console.WriteLine("Invalid Code Word");
        Console.WriteLine("Default to code word \"code\"");
        code = "code";
    }

    for (int i = code.Length - 1; i >= 0; i--) {
        hexList.Insert(0, code[i]);
    }

    int[] change = new int[code.Length];

    Random rnd = new Random();

    for (int i = 0; i < change.Length; i++) {
        change[i] = rnd.Next(1, 254);
    }

    for (int i = 0; i < hexList.Count; i ++) {
        hexList[i] = Wrap(hexList[i] + change[Wrap(i, 0, change.Length - 1)], 0, 255); 
    }

    byte[] byteArray = new byte[hexList.Count];

    for (int i = 0; i < hexList.Count; i++) {
        byteArray[i] = Convert.ToByte(hexList[i].ToString("X2"), 16);
    }

    return byteArray;
}

//Decrypting the data in the file
static byte[] Decrypt(string[] hex) {
    List<int> hexList = new List<int>();

    for (int i = 0; i < hex.Length; i++) {
        hexList.Add(Convert.ToInt32(hex[i], 16));
    }

    Console.Write("Give your secret code word: ");
    string? code = Console.ReadLine();

    if (code == null) {
        Console.WriteLine("Invalid Code Word");
        Console.WriteLine("Default to code word \"code\"");
        code = "code";
    }

    int[] change = new int[code.Length];

    for (int i = 0; i < code.Length; i ++) {
        int index = 0;
        while (Wrap(code[i] + index, 0, 255) != hexList[i]) {
            index++;
        }
        change[i] = index;
    }

    for (int i = 0; i < change.Length; i++) {
        hexList.RemoveAt(0);
    }

    for (int i = 0; i < hexList.Count; i ++) {
        hexList[i] = Wrap(hexList[i] - change[Wrap(i, 0, change.Length - 1)], 0, 255); 
    }

    byte[] byteArray = new byte[hexList.Count];

    for (int i = 0; i < hexList.Count; i++) {
        byteArray[i] = Convert.ToByte(hexList[i].ToString("X2"), 16);
    }

    return byteArray;
}



//Code starts here

//Reciving User input
Console.Clear();

Console.WriteLine("Do you want to encrypt or decrypt a file? e/d");

string? resp = Console.ReadLine();

string path = FindPath();

byte[] bytes = File.ReadAllBytes(path);

string message = BitConverter.ToString(bytes);

Console.WriteLine(message);

string[] splitBytes = message.Split('-');

if (resp == "e") {

    File.WriteAllBytes(path, Encrypt(splitBytes));

    Console.WriteLine("Message Successfully Encrypted");

} else if (resp == "d") {

    File.WriteAllBytes(path, Decrypt(splitBytes));

    Console.WriteLine("Message Successfully Decrypted");

} else if (resp == null)
    return;
