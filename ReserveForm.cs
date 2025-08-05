using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;


namespace radar_settinf_tool_project
{
    public class ReserveForm : Form
    {
        private List<(string serverip, string uid, int port, string command, string value)> failSendList;

        private RichTextBox displayBox;

        private Label dateLabel;
        private DateTimePicker datePicker;

        private Label hourLabel;
        private DateTimePicker hourPicker;

        private Label minuteLabel;
        private NumericUpDown minutePicker;

        private Button confirmButton;

        private Timer checkTimer;
        private DateTime selectedTime;

        public ReserveForm(List<(string serverip, string uid, int port, string command, string value)> failSendList)
        {
            this.failSendList = failSendList;

            this.Text = "예약 설정";
            this.Size = new Size(550, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            displayBox = new RichTextBox
            {
                Dock = DockStyle.Top,
                ReadOnly = true,
                Height = 200,
                Font = new Font("Consolas", 10),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BackColor = Color.White
            };

            foreach (var item in failSendList)
            {
                displayBox.AppendText($"UID: {item.uid}, Port: {item.port}, Command: {item.command}, Value: {item.value}\n");
            }

            // 날짜 라벨과 Picker
            dateLabel = new Label
            {
                Text = "예약 날짜",
                Location = new Point(20, 210),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.Date,
                Location = new Point(100, 205),
                Width = 120,
                Font = new Font("맑은 고딕", 10)
            };

            // 시간 라벨과 Picker
            hourLabel = new Label
            {
                Text = "예약 시간",
                Location = new Point(240, 210),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            hourPicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH",   // 시간만 표시
                ShowUpDown = true,
                Value = DateTime.Now,
                Location = new Point(320, 205),
                Width = 50,
                Font = new Font("맑은 고딕", 10)
            };

            // 분 라벨과 NumericUpDown
            minuteLabel = new Label
            {
                Text = "예약 분",
                Location = new Point(380, 210),
                AutoSize = true,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            minutePicker = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 59,
                Value = DateTime.Now.Minute,
                Location = new Point(450, 205),
                Width = 60,
                Font = new Font("맑은 고딕", 10)
            };

            // 확인 버튼
            confirmButton = new Button
            {
                Text = "확인",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                BackColor = Color.LightSteelBlue
            };
            confirmButton.Click += ConfirmButton_Click;

            // 타이머 초기화
            checkTimer = new Timer();
            checkTimer.Interval = 1000; // 1초마다 체크
            checkTimer.Tick += async (s, e) => await CheckTimer_Tick(s, e);


            // 컨트롤 추가
            this.Controls.Add(displayBox);

            this.Controls.Add(dateLabel);
            this.Controls.Add(datePicker);

            this.Controls.Add(hourLabel);
            this.Controls.Add(hourPicker);

            this.Controls.Add(minuteLabel);
            this.Controls.Add(minutePicker);

            this.Controls.Add(confirmButton);
        }

        private Socket InitSocket(string server_ip, int server_port)
        {
            // 소켓 생성 및 연결
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // IPv4, 스트림 소켓 (TCP), TCP 프로토콜
                socket.Connect(server_ip, server_port);
                return socket;
            }
            catch (Exception ex)
            {
                Console.WriteLine("연결 실패: " + ex.Message);
                //AppendLog($"연결 실패:  + {ex.Message}");
                return null;
            }
        }

        private async Task SendAllFallDataAsync()
        {
            StringBuilder resultSummary = new StringBuilder(); // 결과 저장용

            foreach (var item in failSendList)
            {
                Socket sock = null;

                try
                {
                    var connectTask = Task.Run(() => InitSocket(item.serverip, item.port));
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        string msg = $"UID {item.uid} 포트 {item.port}: 연결 타임아웃 (10초)";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                        continue;
                    }

                    sock = connectTask.Result;

                    if (sock == null)
                    {
                        string msg = $"UID {item.uid} 포트 {item.port}: 소켓 연결 실패";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                        continue;
                    }

                    var response = await Task.Run(() => SendControlMessage(sock, item.uid, item.command, item.value));
                    string status = response.ContainsKey("status") ? response["status"]?.ToString() : null;
                    string type = response.ContainsKey("type") ? response["type"]?.ToString() : null;

                    if (status == "success" || type == "cli")
                    {
                        string msg = $"UID {item.uid} 포트 {item.port} command:{item.command} value:{item.value} - 명령 전송 성공";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                    }
                    else
                    {
                        string msg = $"UID {item.uid} 포트 {item.port} command:{item.command} value:{item.value} - 명령 전송 실패";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                    }
                }
                catch (Exception ex)
                {
                    string msg = $"UID {item.uid} 포트 {item.port} 예외 발생: {ex.Message}";
                    Console.WriteLine(msg);
                    resultSummary.AppendLine(msg);
                }
                finally
                {
                    if (sock != null && sock.Connected)
                    {
                        try
                        {
                            sock.Shutdown(SocketShutdown.Both);
                            sock.Close();
                        }
                        catch { }
                    }
                }
            }
            

            string summary = resultSummary.ToString();

            // UI 요소 접근 전 체크
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        displayBox.AppendText($"\n[예약 실행 결과 - {DateTime.Now:HH:mm}]\n");
                        displayBox.AppendText(summary);
                        displayBox.ScrollToCaret();
                    }));
                }
                catch (ObjectDisposedException)
                {
                    // 이미 폼이 닫혔다면 무시
                }
            }

            // 결과는 항상 MessageBox로 보여주기 (안정적)
            //ShowResultMessageBox(summary);
        }

        private async Task SendSaveToUid()
        {
            StringBuilder resultSummary = new StringBuilder(); // 결과 저장용

            foreach (var item in failSendList)
            {
                Socket sock = null;

                try
                {
                    var connectTask = Task.Run(() => InitSocket(item.serverip, item.port));
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        string msg = $"UID {item.uid} 포트 {item.port}: 연결 타임아웃 (10초)";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                        continue;
                    }

                    sock = connectTask.Result;

                    if (sock == null)
                    {
                        string msg = $"UID {item.uid} 포트 {item.port}: 소켓 연결 실패";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                        continue;
                    }

                    string command = "save";
                    string value = "";

                    var response = await Task.Run(() => SendControlMessage(sock, item.uid, command, value));
                    string status = response.ContainsKey("status") ? response["status"]?.ToString() : null;
                    string type = response.ContainsKey("type") ? response["type"]?.ToString() : null;

                    if (status == "success" || type == "cli")
                    {
                        string msg = $"UID {item.uid} 포트 {item.port} command:{item.command} value:{item.value} - 명령 전송 성공";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                    }
                    else
                    {
                        
                        string msg = $"UID {item.uid} 포트 {item.port} command:{item.command} value:{item.value} - 명령 전송 실패";
                        Console.WriteLine(msg);
                        resultSummary.AppendLine(msg);
                    }
                }
                catch (Exception ex)
                {
                    string msg = $"UID {item.uid} 포트 {item.port} 예외 발생: {ex.Message}";
                    Console.WriteLine(msg);
                    resultSummary.AppendLine(msg);
                }
                finally
                {
                    if (sock != null && sock.Connected)
                    {
                        try
                        {
                            sock.Shutdown(SocketShutdown.Both);
                            sock.Close();
                        }
                        catch { }
                    }
                }
            }
            

            string summary = resultSummary.ToString();

            // UI 요소 접근 전 체크
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        displayBox.AppendText($"\n[예약 실행 결과 - {DateTime.Now:HH:mm:ss}]\n");
                        displayBox.AppendText(summary);
                        displayBox.ScrollToCaret();
                    }));
                }
                catch (ObjectDisposedException)
                {
                    // 이미 폼이 닫혔다면 무시
                }
            }

            // 결과는 항상 MessageBox로 보여주기 (안정적)
            ShowResultMessageBox(summary);
        }


        private void ShowResultMessageBox(string result)
        {
            string title = $"예약 실행 결과 - {DateTime.Now:HH:mm:ss}";
            MessageBox.Show($"{title}\n\n{result}", "전송 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private Dictionary<string, object> SendControlMessage(Socket clientSocket, string uid, string command, string value)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (clientSocket == null || !clientSocket.Connected)
                {
                    result["status"] = "error";
                    result["error"] = "Socket is not initialized or not connected.";
                    return result;
                }

                // 5초 수신 타임아웃 설정
                clientSocket.ReceiveTimeout = 5000;

                // Control 메시지 생성
                var message = new Dictionary<string, string>
                {
                    { "type", "control" },
                    { "uid", uid },
                    { "command", command },
                    { "value", value }
                };

                // JSON 직렬화 후 전송
                string jsonMessage = JsonConvert.SerializeObject(message);
                byte[] dataToSend = Encoding.UTF8.GetBytes(jsonMessage);
                clientSocket.Send(dataToSend);

                // save 명령은 서버 재시작으로 응답이 없으므로 성공으로 간주
                if (command == "save")
                {
                    Console.WriteLine("save 명령어 입력 - 응답 없이 성공 처리");
                    result["status"] = "success";
                    result["note"] = "Response skipped due to 'save' command.";
                    return result;
                }

                // 응답 수신 (최대 5초 대기)
                byte[] buffer = new byte[8192];
                int received = clientSocket.Receive(buffer);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, received);

                // 응답 JSON 파싱
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                return responseDict;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                result["status"] = "error";
                result["error"] = "Receive timeout (no response from server).";
                return result;
            }
            catch (Exception ex)
            {
                result["status"] = "error";
                result["error"] = ex.Message;
                return result;
            }
        }




        /*
        private void AppendLog(string message)
        {
            if (resultBox == null) return;

            Color color = Color.Black;
            if (message.Contains("성공"))
            {
                color = Color.LimeGreen;
            }
            else if (message.Contains("실패") || message.Contains("Error")|| message.Contains("not"))
            {
                color = Color.Red;
            }

            resultBox.SelectionStart = resultBox.TextLength;
            resultBox.SelectionLength = 0;

            resultBox.SelectionColor = color;
            resultBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            resultBox.SelectionColor = resultBox.ForeColor;

            resultBox.ScrollToCaret(); // 자동 스크롤
        }
        */
        // 여기서부터 이벤트 핸들러 
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            DateTime date = datePicker.Value.Date;
            int hour = hourPicker.Value.Hour;
            int minute = (int)minutePicker.Value;

            selectedTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
            DateTime now = DateTime.Now;

            if (selectedTime <= now)
            {
                MessageBox.Show("설정한 시간이 현재 시간보다 이전입니다.\n미래 시간을 선택해주세요.", "시간 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"예약시간 \"{selectedTime:yyyy-MM-dd HH:mm}\"로 설정되었습니다.\n예약 시간이 되면 메시지가 출력됩니다.", "예약 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            

            // 타이머 시작
            checkTimer.Start();
            this.Close();
        }

        private async Task CheckTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            if (now >= selectedTime)
            {
                Console.WriteLine($"예약 시간이 되었습니다! ({selectedTime:yyyy-MM-dd HH:mm})");
                checkTimer.Stop();

                // 필요하면 UI 알림 추가 가능
                //MessageBox.Show($"예약 시간이 되었습니다! ({selectedTime:yyyy-MM-dd HH:mm})", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 세팅 함수
                await SendAllFallDataAsync();
                await SendSaveToUid();
                
            }
        }
    }

}
