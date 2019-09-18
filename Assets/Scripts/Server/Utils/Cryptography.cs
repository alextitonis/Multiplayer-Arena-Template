namespace Server
{
    public class Cryptography
    {
        public static string Encrypt_Custom(string input)
        {
            string output = "";

            foreach (var letter in input)
            {
                switch (letter)
                {
                    case 'A':
                        output += 'D';
                        break;
                    case 'B':
                        output += 'F';
                        break;
                    case 'C':
                        output += 'H';
                        break;
                    case 'D':
                        output += 'A';
                        break;
                    case 'E':
                        output += 'G';
                        break;
                    case 'F':
                        output += 'B';
                        break;
                    case 'G':
                        output += 'E';
                        break;
                    case 'H':
                        output += 'C';
                        break;
                    case 'I':
                        output += 'K';
                        break;
                    case 'J':
                        output += 'N';
                        break;
                    case 'K':
                        output += 'I';
                        break;
                    case 'L':
                        output += 'P';
                        break;
                    case 'M':
                        output += 'R';
                        break;
                    case 'N':
                        output += 'J';
                        break;
                    case 'O':
                        output += 'U';
                        break;
                    case 'P':
                        output += 'L';
                        break;
                    case 'Q':
                        output += 'Z';
                        break;
                    case 'R':
                        output += 'M';
                        break;
                    case 'S':
                        output += 'Y';
                        break;
                    case 'T':
                        output += 'X';
                        break;
                    case 'U':
                        output += 'O';
                        break;
                    case 'V':
                        output += 'W';
                        break;
                    case 'W':
                        output += 'V';
                        break;
                    case 'X':
                        output += 'T';
                        break;
                    case 'Y':
                        output += 'S';
                        break;
                    case 'Z':
                        output += 'Q';
                        break;

                    case 'a':
                        output += 'd';
                        break;
                    case 'b':
                        output += 'f';
                        break;
                    case 'c':
                        output += 'h';
                        break;
                    case 'd':
                        output += 'a';
                        break;
                    case 'e':
                        output += 'g';
                        break;
                    case 'f':
                        output += 'b';
                        break;
                    case 'g':
                        output += 'e';
                        break;
                    case 'h':
                        output += 'c';
                        break;
                    case 'i':
                        output += 'k';
                        break;
                    case 'j':
                        output += 'l';
                        break;
                    case 'k':
                        output += 'i';
                        break;
                    case 'l':
                        output += 'p';
                        break;
                    case 'm':
                        output += 'r';
                        break;
                    case 'n':
                        output += 'j';
                        break;
                    case 'o':
                        output += 'u';
                        break;
                    case 'p':
                        output += 'l';
                        break;
                    case 'q':
                        output += 'z';
                        break;
                    case 'r':
                        output += 'm';
                        break;
                    case 's':
                        output += 'y';
                        break;
                    case 't':
                        output += 'x';
                        break;
                    case 'u':
                        output += 'o';
                        break;
                    case 'v':
                        output += 'w';
                        break;
                    case 'w':
                        output += 'v';
                        break;
                    case 'x':
                        output += 't';
                        break;
                    case 'y':
                        output += 's';
                        break;
                    case 'z':
                        output += 'q';
                        break;

                    case '0':
                        output += '9';
                        break;
                    case '1':
                        output += '4';
                        break;
                    case '2':
                        output += '7';
                        break;
                    case '3':
                        output += '8';
                        break;
                    case '4':
                        output += '1';
                        break;
                    case '5':
                        output += '6';
                        break;
                    case '6':
                        output += '5';
                        break;
                    case '7':
                        output += '2';
                        break;
                    case '8':
                        output += '3';
                        break;
                    case '9':
                        output += '0';
                        break;

                    default:
                        output += letter;
                        break;
                }
            }

            return output;
        }
    }
}