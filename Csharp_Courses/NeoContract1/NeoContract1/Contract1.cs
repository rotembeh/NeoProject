using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;

using System;
using System.Numerics;

namespace NeoContract1
{
    public class Contract1 : SmartContract
    {
            
     //     private static readonly byte[] uniPublic_keyStr = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y".ToScriptHash();
       /*     static int passwordLen = 3;
            static int courseAmount = 3;
            static int studentsAmount = 10;
            static int checkTime = 0;*/
        //Params: args[0]- Wanted Course Number. args[1]- Student ID. 
        //args[2]- Offering Course. args[3]- Password.
        //Return: on error: "Last Operation" value with error code. 
        //on queries: 0 - false. 1 - true or success.
        //on unknown operation: "Last Operation" value.
        //071007 (returns 07)
        public static String Main(String operation, String[] args)
        {
            int checkTime = 0;
            Runtime.Log("msgLog: Invoking Contract");
            Runtime.Notify("msgNotify: Invoking Contract");
            String password = "";
            if (args.Length > 3)
                password = args[3];

            if (checkTime == 1) { //allowing operation after start time 
                uint STARTREG = 0;// here we can choose the start time of the registration
                Header header = Blockchain.GetHeader(Blockchain.GetHeight());
                if (header.Timestamp < STARTREG) //|| header.Timestamp > ENDREG)
                { // 2018-6-6 18:10
                    Runtime.Log("time fault");
                    return "Reg is off now";
                }
            }
            
            if (operation == "refresh")
            {
                String coursesStatus = writeStatus(args);
                return coursesStatus;
            }

            if (operation == "initTable")
            {
                /*         if (Runtime.CheckWitness(uniPublic_keyStr))
                             initTable(); //only uni account can init the table.
                         else
                             return "check Witness Failed";*/
                initTable();
            }

            if (operation == "CheckAvailable")
            {
                byte[] ans = CheckAvailable(args);
                if (ans != null)
                    return ans.AsString();
            }

            if (operation == "regReq")
            {
                if (checkPass(args, password) == -1)
                    return "Wrong Password";
                byte[] ans = regReq(args);
                if (ans != null)
                    return ans.AsString();
            }

            if (operation == "view") //(view course ID's list)
            {
                if (checkPass(args, password) == -1)
                    return "Wrong Password";
                String res = "ID -";
                res += args[1];
                res += "- courses: ";
                String IDcourses = Storage.Get(Storage.CurrentContext, args[1]).AsString();
                res += IDcourses;
                return res;
            }

            if (operation == "view2") //(view course's IDs list)
            {
                String res = "Course -";
                res += args[0];
                res += "- IDs: ";
                String IDcourses = Storage.Get(Storage.CurrentContext, args[0]).AsString();
                res += IDcourses;
                return res;
            }

            if (operation == "remove") 
            {
                if (checkPass(args, password) == -1)
                    return "Wrong Password";
                byte[] ans = remove(args);
                if (ans != null)
                    return ans.AsString();
            }

            if (operation == "errorInfo")
            {
                return Storage.Get(Storage.CurrentContext, "errorInfo").AsString();
            }

            if (operation == "errorInfo2")
            {
                return Storage.Get(Storage.CurrentContext, "errorInfo2").AsString();
            }

            if (operation == "addStudent")
            {
                /*         if (Runtime.CheckWitness(uniPublic_keyStr))
             addStudent(args); //only uni account can add new student.
                else
             return "not uni address";*/
                byte[] ans = addStudent(args);
                return ans.AsString();
            }

            if (operation == "offer")
            {
                if (checkPass(args, password) == -1)
                    return "Wrong Password";
                byte[] ans = offer(args);
        //        if (ans != null)
         //           return ans.AsString();
            }
            return Storage.Get(Storage.CurrentContext, "Last operation").AsString();
        }

        //in practic nodejs will send the details to build the initTable.
        //including courses, IDs, passwords.
        public static void initTable() 
        {
            //course's list, amount and capacity:
            Storage.Put(Storage.CurrentContext, "1", "");
            Storage.Put(Storage.CurrentContext, "1-Amount", "0");
            Storage.Put(Storage.CurrentContext, "1-Capacity", "10");
            Storage.Put(Storage.CurrentContext, "2", "");
            Storage.Put(Storage.CurrentContext, "2-Amount", "0");
            Storage.Put(Storage.CurrentContext, "2-Capacity", "20");
            Storage.Put(Storage.CurrentContext, "3", "");
            Storage.Put(Storage.CurrentContext, "3-Amount", "0");
            Storage.Put(Storage.CurrentContext, "3-Capacity", "2");

            //course points:
            Storage.Put(Storage.CurrentContext, "1-Points", "2");
            Storage.Put(Storage.CurrentContext, "2-Points", "2");
            Storage.Put(Storage.CurrentContext, "3-Points", "2");

            //id's courses:
            Storage.Put(Storage.CurrentContext, "111", "");
            Storage.Put(Storage.CurrentContext, "222", "");
            Storage.Put(Storage.CurrentContext, "333", "");
            Storage.Put(Storage.CurrentContext, "444", "");
            Storage.Put(Storage.CurrentContext, "555", "");
            Storage.Put(Storage.CurrentContext, "666", "");
            Storage.Put(Storage.CurrentContext, "777", "");
            Storage.Put(Storage.CurrentContext, "888", "");
            Storage.Put(Storage.CurrentContext, "999", "");


            //available points: (Counting points is done. minus points is permitted).
            Storage.Put(Storage.CurrentContext, "111-a", "8");
            Storage.Put(Storage.CurrentContext, "222-a", "8");
            Storage.Put(Storage.CurrentContext, "333-a", "8");
            Storage.Put(Storage.CurrentContext, "444-a", "8");
            Storage.Put(Storage.CurrentContext, "555-a", "8");
            Storage.Put(Storage.CurrentContext, "666-a", "8");
            Storage.Put(Storage.CurrentContext, "777-a", "8");
            Storage.Put(Storage.CurrentContext, "888-a", "8");
            Storage.Put(Storage.CurrentContext, "999-a", "8");

            //passwords:
            Storage.Put(Storage.CurrentContext, "111-p", "11");
            Storage.Put(Storage.CurrentContext, "222-p", "22");
            Storage.Put(Storage.CurrentContext, "333-p", "33");
            Storage.Put(Storage.CurrentContext, "444-p", "44");
            Storage.Put(Storage.CurrentContext, "555-p", "55");
            Storage.Put(Storage.CurrentContext, "666-p", "66");
            Storage.Put(Storage.CurrentContext, "777-p", "77");
            Storage.Put(Storage.CurrentContext, "888-p", "88");
            Storage.Put(Storage.CurrentContext, "999-p", "99");

            Storage.Put(Storage.CurrentContext, "Last operation", "initTable success");
            Runtime.Log("Init Table Success");
        }

        private static int checkPass(String[] args, String password)
        {
            byte[] inputValidity = checkInput(args);
       /*     if (inputValidity != null) //input not valid
            {
                return -1;
            }*/
            String ID = args[1];
            String idPassKey = ID;
            idPassKey += "-p";
            byte[] idPass = Storage.Get(Storage.CurrentContext, idPassKey);
            byte[] passwordArr = password.AsByteArray();
            if (cmpByteArrays(idPass, passwordArr) != 1)
            {
                Runtime.Log("CheckPass Faild");
                return -1;
            }
            Runtime.Log("CheckPass Success");
            return 1;
        }

        //returns 1 if equals, else -1.
        private static int cmpByteArrays(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return -1;
            for (int k = 0; k < arr1.Length; k++)
            {
                byte[] dig1 = arr1.Range(k, 1);
                byte[] dig2 = arr2.Range(k, 1);
                if (BytesToInt(dig1) != BytesToInt(dig2))
                    return -1;
            }
                return 1;
        }

        //check if course is available. 
        //returns null if input not valid (error). else, returns 0 if course is full or 1 else.
        //courseNum (args[0]).
        public static byte[] CheckAvailable(String[] args)
        {
            if (args.Length < 1)
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "checkAvailable error 1");
                return null;
            }

            byte[] inputValidity = checkInput(args);
            if (inputValidity != null) //input not valid
            {
        //        Runtime.Notify(inputValidity);
                return null;
            }
            String courseNum = args[0];
            String courseAmountKey = courseNum + "-Amount";
            String courseCapacityKey = courseNum + "-Capacity";
            byte[] amount = Storage.Get(Storage.CurrentContext, courseAmountKey);
            byte[] capacity = Storage.Get(Storage.CurrentContext, courseCapacityKey);
            if (BytesToInt(amount) >= BytesToInt(capacity)) { //course is full
                String ret = "Course ";
                ret += courseNum;
                ret += " is full.";
                Storage.Put(Storage.CurrentContext, "errorInfo", "checkAvailable error 2 (course is full)");
                return ret.AsByteArray(); //ret length is 15-18 (to identify full course)
         //       return null;
            }
            else
            {
                String ret = "Course ";
                ret += courseNum;
                ret += " status: ";
                String amountStr = amount.AsString();
                String capacityStr = capacity.AsString();
                ret += amountStr;
                ret += "/";
                ret += capacity;
                ret += ". Available.";
                Storage.Put(Storage.CurrentContext, "Last operation", "checkAvailable: course is available");
                return ret.AsByteArray(); //ret length is > 20
            }
        }

        //checks courseNum, ID.
        //returns null if ok, else returns error input and updates "Last operation".
        public static byte[] checkInput (String[] args)
        {
            String courseNum = args[0];
            String ID = args[1];
            if (ID.Length != 3 && ID.Length != 0)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "checkAvailable error 2 (id length)");
                return "ID length error".AsByteArray();
            } //COURSE AMOUNT :
            byte[] courseAmount = "3".AsByteArray();
            if (args.Length >= 3 && args[2].Length > 0)
            {
                byte[] offerCourseNumBytes = args[2].AsByteArray();
                if (BytesToInt(offerCourseNumBytes) > BytesToInt(courseAmount))
                {
                    Storage.Put(Storage.CurrentContext, "Last operation", "checkAvailable error 3 (offer course num length)");
                    return "offer courseNum length error".AsByteArray();
                }
            }

            byte[] courseNumBytes = courseNum.AsByteArray();
            if (BytesToInt(courseNumBytes) > BytesToInt(courseAmount)) 
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "checkAvailable error 4 (course num value error)");
                return "courseNum value error".AsByteArray();
            }
            return null;
        }

        public static byte[] regReq(String[] args)
        {
            if (checkInput(args) != null)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "regReq error 1 (check the variables)");
                return null;
            }
            byte[] available = CheckAvailable(args);
            if (available == null || (available.Length > 15 && available.Length < 19))
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "regReq error 2 (Course not available)");
                return null;
            }
            int check = CheckIfInCourse(args[1].AsByteArray(), args[0].AsByteArray());
            if (check >= 0 || check == -2) //ID already signed to that course, -2 is fault
            {
    //            Storage.Put(Storage.CurrentContext, "errorInfo2", "regReq errorInfo2");
                Storage.Put(Storage.CurrentContext, "Last operation", "regReq error 3 (you already signed to the course)");
                return null;
            }
            String courseNum = args[0];
            String ID = args[1];
            String members = Storage.Get(Storage.CurrentContext, courseNum).AsString();
            members += "|";
            members += ID;
            Storage.Put(Storage.CurrentContext, courseNum, members);
            incAmount(courseNum);
            String IDcourses = Storage.Get(Storage.CurrentContext, ID).AsString();
            IDcourses += "|";
            IDcourses += courseNum;
            Storage.Put(Storage.CurrentContext, ID, IDcourses);
            subPointsToID(ID, courseNum);    
            String Log = "Registering ID ";
            Log += ID;
            Log += " to course ";
            Log += courseNum;
            Log += "...";
            Runtime.Log(Log);
       //     Storage.Put(Storage.CurrentContext, "Last operation", "regReq success");
            Storage.Put(Storage.CurrentContext, "Last operation", Log);
            return Log.AsByteArray();
      //      return "1".AsByteArray();
        }

        //removes ID from courseNum.
        //returns 
        public static byte[] remove(String[] args)
        {
            if (checkInput(args) != null)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "remove error 1");
                return null;
            }
            String courseNum = args[0];
            String ID = args[1];
            String members = Storage.Get(Storage.CurrentContext, courseNum).AsString();
            int ans = clearID(members, courseNum, ID);
            if (ans == 1)
            {
                decAmount(courseNum);
                String Log = "Removing ID ";
                Log += ID;
                Log += " from course ";
                Log += courseNum;
                Log += "...";
                Runtime.Log(Log);
                //        Storage.Put(Storage.CurrentContext, "Last operation", "remove success");
                Storage.Put(Storage.CurrentContext, "Last operation", Log);
                return Log.AsByteArray();
              //  return "1".AsByteArray();
            }
            if (ans == -1)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "remove error 2");
                return "error".AsByteArray();
            }
            //not found: (ans == 0)
            Storage.Put(Storage.CurrentContext, "Last operation", "remove error 3 (not in course)");
            return "remove error 3 (id not in course)".AsByteArray();
        }

        //returns 1 on success. 0 if not found. -1 on error.
        private static int clearID(String members, String courseNum, String ID)
        {
            byte[] mem_arr = members.AsByteArray();
            byte[] ID_arr = ID.AsByteArray();
            int idx = searchID(mem_arr, ID_arr, 1);
            if (idx == -2)
                return -1;
            if (idx == -1) //was 0.
                return 0;
            int ret = deleteIDfromStorage(mem_arr, ID_arr, courseNum, idx);
            if (ret == -1)
                return -1;
            if (ret == 1)
                return 1; //success
            Storage.Put(Storage.CurrentContext, "errorInfo", "clearID value error");
            return -1;
        }

        //deletes ID from value of courseNum, and deletes courseNum from value of ID.
        //returns -1 on error and 1 on success
        private static int deleteIDfromStorage(byte[] mem_arr, byte[] ID_arr, String courseNum, int idx)
        {
            if (idx < 1)
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "deleteIDfromStorage error 1");
                return -1;
            }
            //TODO: check cases: last, first, last and first (only one).
            if (mem_arr.Length == 4) //if there is only one ID in that course.
            {
                if (idx != 1) //idx must be 1
                {
                    Storage.Put(Storage.CurrentContext, "errorInfo", "deleteIDfromStorage error 2");
                    return -1;
                }
                Storage.Put(Storage.CurrentContext, courseNum, "");
            }
            else
            {
                byte[] new_mem2;
                byte[] new_mem1 = mem_arr.Range(0, idx - 1);
                if (idx == mem_arr.Length - 3) //ID is last in mem_arr
                    new_mem2 = "".AsByteArray();                
                else
                    new_mem2 = mem_arr.Range(idx + 3, mem_arr.Length - (idx + 3));
                String new_mem_str = new_mem1.AsString() + new_mem2.AsString();
                Storage.Put(Storage.CurrentContext, courseNum, new_mem_str);
            }
            //also delete from the ID value:::
            byte[] ID_val_arr = Storage.Get(Storage.CurrentContext, ID_arr.AsString());
            byte[] courseNum_arr = courseNum.AsByteArray();
            int idx2 = searchID(ID_val_arr, courseNum_arr, 2);
            if (idx2 == -1 || idx2 == -2) //error. IDval should contain courseNum
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "deleteIDfromStorage error 3");
                return -1;
            }
            if (ID_val_arr.Length == 2) //this ID is signed only to one course
            {
                Storage.Put(Storage.CurrentContext, ID_arr.AsString(), "");
            }
            else
            {
                byte[] new_idVal2;
                byte[] new_idVal1 = ID_val_arr.Range(0, idx2 - 1);
                if (idx2 == ID_val_arr.Length - 1) //courseNum is last in ID_val (ID courses)
                    new_idVal2 = "".AsByteArray();
                else
                    new_idVal2 = ID_val_arr.Range(idx2 + 1, mem_arr.Length - (idx + 1));
                String new_idVal_str = new_idVal1.AsString() + new_idVal2.AsString();
                Storage.Put(Storage.CurrentContext, ID_arr.AsString(), new_idVal_str);
            }
            //adding the points to id
            String IDstr = ID_arr.AsString();
            addPointsToID(IDstr, courseNum);
            return 1;
        }

        //adding points after remove
        private static void addPointsToID(String ID, String courseNum)
        {
            String pointsKey = ID;
            pointsKey += "-a";
            byte[] points = Storage.Get(Storage.CurrentContext, pointsKey);
            BigInteger intPoints = points.AsBigInteger();
            String coursePointKey = courseNum;
            coursePointKey += "-points";
            byte[] pointsToAdd = Storage.Get(Storage.CurrentContext, coursePointKey);
            BigInteger toAdd = pointsToAdd.AsBigInteger();
            BigInteger newVal = intPoints + toAdd;
            byte[] newValArr = newVal.AsByteArray();
            Storage.Put(Storage.CurrentContext, pointsKey, newValArr);
        }

        //sub points after reg
        private static void subPointsToID(String ID, String courseNum)
        {
            String pointsKey = ID;
            pointsKey += "-a";
            byte[] points = Storage.Get(Storage.CurrentContext, pointsKey);
            BigInteger intPoints = points.AsBigInteger();
            String coursePointKey = courseNum;
            coursePointKey += "-points";
            byte[] pointsToSub = Storage.Get(Storage.CurrentContext, coursePointKey);
            BigInteger toSub = pointsToSub.AsBigInteger();
            BigInteger newVal = intPoints - toSub;
            byte[] newValArr = newVal.AsByteArray();
            Storage.Put(Storage.CurrentContext, pointsKey, newValArr);
        }

        //input: mode 1: arr(as string): xxx|yyy|qqq|zxc... , pattern: yyy.
        //mode 3: example: str= "|2-123|5-777", pat= "5" -> returns: 5-777.
        //returns: index of pattern in arr. if not found returns -1. on error -2.
        private static int searchID(byte[] arr, byte[] pattern, int mode)
        {
            if (arr.Length == 0)
                return -1;
            int patternLength = -1;
            if (mode == 1)
                patternLength = 3; // arr: |xxx|yyy... pat: xxx
            if (mode == 2)
                patternLength = 1; // arr: |x|y|z... pat: x
            if (mode == 3)
                patternLength = 5; // arr: |2-123|5-777 pat: 5. note that here pattern real length is 1

            if (mode == 1 || mode == 2 || mode == 3) {
                patternLength++;
                int i = 0;
                for (i = 0; i <= arr.Length - patternLength; i += patternLength) //search
                {
                    if (arr[i] == (byte)124) //7c = 124 =0 '|'
                    {
                        int equals = 1;
                        for (int k = 0; k < pattern.Length; k++)
                        {
                            byte[] memdig = arr.Range(i + k + 1, 1);
                            byte[] IDdig = pattern.Range(k, 1);
                            if (BytesToInt(IDdig) != BytesToInt(memdig))
                                equals = 0;
                        }
                        if (equals == 1)
                        {
                            return i + 1;
                        }
                    }
                    else // not '|' -> shoudn't accours. error in mode or arr.
                    {
                        Storage.Put(Storage.CurrentContext, "errorInfo", "searchID error 1");
                        return -2;
                    }
                }
                return -1; //not found
            }
            //mode number not exist
            Storage.Put(Storage.CurrentContext, "errorInfo", "searchID error 2");
            return -2;
        }

        //ID offering "offeredCourse" for "wantedCourse".
        public static byte[] offer(String[] args)
        {
            if (checkInput(args) != null)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "offer error 1");
                return null;
            }
            String wantedCourse = args[0];
            byte[] wantedCourse_arr = wantedCourse.AsByteArray();
            String ID = args[1];
            String offeredCourse = args[2];
            //checks that ID really have the offeredCourse.
            int verify = CheckIfInCourse(ID.AsByteArray(), offeredCourse.AsByteArray());
            if (verify == -1 || verify == -2)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "offer course error 1");
                return null;
            }
            verify = CheckIfInCourse(ID.AsByteArray(), wantedCourse_arr);
            //checks that ID really don't have the wanted course
            if (verify != -1 || verify == -2)
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "offer course error 2");
                return null;
            }
            String offeredCourseKey = "deal-" + offeredCourse;
            byte[] checklist = Storage.Get(Storage.CurrentContext, offeredCourseKey); //list of who wants what ID offers
            int idx = searchID(checklist, wantedCourse_arr, 3); //checks if someone offered the wanted course for the offered one
            if (idx == -2) //error
            {
                Storage.Put(Storage.CurrentContext, "Last operation", "offer course error 3");
                return null;
            }
            if (idx == -1) //not found, nobody offered the wanted course and wanted ID's offer
            {
                String wantedCourseKey = "deal-" + wantedCourse;
                String offerVal = "|";
                offerVal += offeredCourse;
                offerVal += "-";
                offerVal += ID;
                String newOfferVal = Storage.Get(Storage.CurrentContext, wantedCourseKey).AsString();
                newOfferVal += offerVal;
                Storage.Put(Storage.CurrentContext, wantedCourseKey, newOfferVal);
                Storage.Put(Storage.CurrentContext, "Last operation", "offer course (new deal)");
            }
            if (idx > -1)
            {
                String course2 = checklist.Range(idx, 1).AsString(); //5 = IDlength(3) + courseNumlength(1) + 1
                String ID2 = checklist.Range(idx + 2, 3).AsString();
                int success = makeSwitch(ID, offeredCourse, ID2, course2);
                if (success == -1 || success == -4)
                {
					if (success == -4){
						//in case that there is a deal that not relevant from some reason
						String wantedCourseKey = "deal-" + wantedCourse;
						String offerVal = "|";
						offerVal += offeredCourse;
						offerVal += "-";
						offerVal += ID;
						String newOfferVal = Storage.Get(Storage.CurrentContext, wantedCourseKey).AsString();
						offerVal += newOfferVal;
						Storage.Put(Storage.CurrentContext, wantedCourseKey, offerVal);
						Storage.Put(Storage.CurrentContext, "Last operation", "offer course (new-deal)");
						return "1".AsByteArray();
					}
					else{ //-1
                      Storage.Put(Storage.CurrentContext, "Last operation", "offer course error 4");
                      return null;
					}
				}
                //else success:
                //clear offer:
                if (checklist.Length == 5)//it was the only offer
                {
                    Storage.Put(Storage.CurrentContext, offeredCourseKey, "");
                }
                else
                {
                    byte[] arr1 = checklist.Range(0, idx - 1);
                    byte[] arr2;
                    if (idx == checklist.Length - 5)
                        arr2 = "".AsByteArray();
                    else
                        arr2 = checklist.Range(idx + 5, checklist.Length - (idx + 5));
                    String newOfferVal = arr1.AsString() + arr2.AsString();
                    Storage.Put(Storage.CurrentContext, offeredCourseKey, newOfferVal);
                }
                Storage.Put(Storage.CurrentContext, "Last operation", "offer course (found match)");
            }
            return "1".AsByteArray();
        }

            //returns -1 if not found or higher if found. -2 on error.
            private static int CheckIfInCourse(byte[] ID, byte[] course)
        {
            byte[] verifyArr = Storage.Get(Storage.CurrentContext, ID);
            int ret = searchID(verifyArr, course, 2);
            if (ret == -2)
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "CheckIfInCourse error 1");
                return -2;
            }
            if (ret == -1)
                Storage.Put(Storage.CurrentContext, "errorInfo2", "CheckIfInCourse returns -1");
            return ret; //-1 if not fount, or higher if found (the index)
        }

        //Switching coureses between two IDs. updating initTable values, and ID1,ID2 values.
        //example: 123, 1, 331, 3. student 123 get 1 and student 331 gets 3
        //return: 1 on success, -1 on error
        private static int makeSwitch(String ID1, String course1, String ID2, String course2)
        {
            byte[] list1 = Storage.Get(Storage.CurrentContext, course1);
            byte[] list2 = Storage.Get(Storage.CurrentContext, course2);
            int idx1 = searchID(list1, ID1.AsByteArray(), 1); //index of ID1 in course1
            int idx2 = searchID(list2, ID2.AsByteArray(), 1); //index of ID2 in course2
            if (idx1 == -2 || idx1 == -1) //not found or error
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "makeSwitch error 1");
                return -4;
            }
            if (idx2 == -2 || idx2 == -1)
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "makeSwitch error 2");
                return -4;
            }
            int success1 = deleteIDfromStorage(list1, ID1.AsByteArray(), course1, idx1);
            int success2 = deleteIDfromStorage(list2, ID2.AsByteArray(), course2, idx2);
            if (success1 == -1)
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "makeSwitch error 3");
                return -1;
            }
            if (success2 == -1)
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "makeSwitch error 4");
                return -1;
            }
            String[] args1 = { course2, ID1 };
            String[] args2 = { course1, ID2 };
            decAmount(course2); //dec amount before trying to add student
            byte[] succ1 = regReq(args1);
            decAmount(course1);
            byte[] succ2 = regReq(args2);
            if (succ1.AsString() == "0")
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "makeSwitch error 5");
                return -1;
            }
            if (succ2.AsString() == "0")
            {
                Storage.Put(Storage.CurrentContext, "errorInfo", "makeSwitch error 6");
                return -1;
            }
            return 1;
        }

        private static void incAmount(String courseNum)
        {
            String key = courseNum + "-Amount";
            byte[] amount = Storage.Get(Storage.CurrentContext, key);
            BigInteger amountInt = BytesToInt(amount);
            amountInt += 1;
            byte[] new_amount = amountInt.AsByteArray();
            Storage.Put(Storage.CurrentContext, key, new_amount);
        }

        private static void decAmount(String courseNum)
        {
            String key = courseNum + "-Amount";
            byte[] amount = Storage.Get(Storage.CurrentContext, key);
            BigInteger amountInt = BytesToInt(amount);
            amountInt -= 1;
            byte[] new_amount = amountInt.AsByteArray();
            Storage.Put(Storage.CurrentContext, key, new_amount);
        }

        //args[1] = ID. args[2] = pass.
        private static byte[] addStudent(String[] args)
        {
            if (Storage.Get(Storage.CurrentContext, args[1]) != null)
                return "student exist".AsByteArray();
            else
            {
                String id_p = args[1];
                id_p += "-p"; //id pass key
                Storage.Put(Storage.CurrentContext, id_p, args[2]);
                String id_a = args[1];
                id_a += "-a"; //available points
                Storage.Put(Storage.CurrentContext, id_a, "8");
            }
            return "Student Added".AsByteArray();
        }

        private static byte[] IntToBytes(BigInteger value)
        {
            byte[] buffer = value.ToByteArray();
            return buffer;
        }

        private static BigInteger BytesToInt(byte[] array)
        {
            BigInteger buffer = new BigInteger(array);
            return buffer;
        }

        //status from args[0]. (default server send 1)
        private static String writeStatus(String[] args)
        {
            if (args.Length < 1)
            {
                return "fault";
            }
            String retstr = "Status: <br>";
            String str_z = args[0]; //"1".
            byte[] zArr = str_z.AsByteArray();
            BigInteger z = BytesToInt(zArr);
            String temp;
            byte[] tempArr;
            for (int j = 0; j < 10; j++)
            {
                tempArr = CheckAvailable(args);
                temp = tempArr.AsString();
                if (temp != null)
                {
                    retstr += temp;
                    retstr += "<br>";
                }
                z += 1;
                zArr = z.AsByteArray();
                str_z = zArr.AsString();
                args[0] = str_z;
            }
            return retstr;
        }
    }
}
