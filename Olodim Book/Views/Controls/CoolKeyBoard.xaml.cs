using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PdfFlipBook.Utilities;
using UserControl = System.Windows.Controls.UserControl;

namespace PdfFlipBook.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для CoolKeyBoard.xaml
    /// </summary>
    public partial class CoolKeyBoard : UserControl
    {
        public CoolKeyBoard()
        {
            InitializeComponent();
            IsEngLanguage = false;
            symbolButton.Tag = "?!@";
            qButton.Tag = "й";
            wButton.Tag = "ц";
            eButton.Tag = "у";
            rButton.Tag = "к";
            tButton.Tag = "е";
            yButton.Tag = "н";
            uButton.Tag = "г";
            iButton.Tag = "ш";
            oButton.Tag = "щ";
            pButton.Tag = "з";
            aButton.Tag = "ф";
            sButton.Tag = "ы";
            dButton.Tag = "в";
            fButton.Tag = "а";
            gButton.Tag = "п";
            hButton.Tag = "р";
            jButton.Tag = "о";
            kButton.Tag = "л";
            lButton.Tag = "д";
            zButton.Tag = "я";
            xButton.Tag = "ч";
            cButton.Tag = "с";
            vButton.Tag = "м";
            bButton.Tag = "и";
            nButton.Tag = "т";
            mButton.Tag = "ь";
            langButton.Tag = "ENG";
            addButton1.Tag = "х";
            addButton2.Tag = "ъ";
            addButton3.Tag = "ж";
            addButton4.Tag = "э";
            addButton5.Tag = "б";
            addButton6.Tag = "ю";
            backButton.Tag = "Удалить";
            //addButton19.Tag= ")";
            //addButton20.Tag="=";
            //addButton7.Tag = "?";


        }

        public static readonly DependencyProperty ChoosedCultureProperty = DependencyProperty.Register(
            "ChoosedCulture", typeof(CultureInfo), typeof(CoolKeyBoard),
            new PropertyMetadata(CultureInfo.GetCultureInfo("ru-Ru")));

        public CultureInfo ChoosedCulture
        {
            get => (CultureInfo) GetValue(ChoosedCultureProperty);
            set => SetValue(ChoosedCultureProperty, value);
        }

        public static readonly DependencyProperty IsEngLanguageProperty = DependencyProperty.Register(
            "IsEngLanguage", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool IsEngLanguage
        {
            get { return (bool) GetValue(IsEngLanguageProperty); }
            set { SetValue(IsEngLanguageProperty, value); }
        }

        public static readonly DependencyProperty CapsPressedProperty = DependencyProperty.Register(
            "CapsPressed", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool CapsPressed
        {
            get => (bool) GetValue(CapsPressedProperty);
            set => SetValue(CapsPressedProperty, value);
        }

        public static readonly DependencyProperty ShiftPressedProperty = DependencyProperty.Register(
            "ShiftPressed", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool ShiftPressed
        {
            get => (bool) GetValue(ShiftPressedProperty);
            set => SetValue(ShiftPressedProperty, value);
        }
        public static event EventHandler FocusWB;

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool IsOpen
        {
            get => (bool) GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        private ICommand _shiftCommand;

        public ICommand ShiftCommand =>
            _shiftCommand ?? (_shiftCommand = new Command(() =>
            {
                //TODO Shift ёпта
                if (CapsPressed)
                {
                    if (IsEngLanguage == true)
                    {
                        qButton.Tag = "Q";
                        wButton.Tag = "W";
                        eButton.Tag = "E";
                        rButton.Tag = "R";
                        tButton.Tag = "T";
                        yButton.Tag = "Y";
                        uButton.Tag = "U";
                        iButton.Tag = "I";
                        oButton.Tag = "O";
                        pButton.Tag = "P";
                        aButton.Tag = "A";
                        sButton.Tag = "S";
                        dButton.Tag = "D";
                        fButton.Tag = "F";
                        gButton.Tag = "G";
                        hButton.Tag = "H";
                        jButton.Tag = "J";
                        kButton.Tag = "K";
                        lButton.Tag = "L";
                        zButton.Tag = "Z";
                        xButton.Tag = "X";
                        cButton.Tag = "C";
                        vButton.Tag = "V";
                        bButton.Tag = "B";
                        nButton.Tag = "N";
                        mButton.Tag = "M";
                        addButton7.Tag = "!";

                    }
                    else
                    {
                        qButton.Tag = "Й";
                        wButton.Tag = "Ц";
                        eButton.Tag = "У";
                        rButton.Tag = "К";
                        tButton.Tag = "Е";
                        yButton.Tag = "Н";
                        uButton.Tag = "Г";
                        iButton.Tag = "Ш";
                        oButton.Tag = "Щ";
                        pButton.Tag = "З";
                        aButton.Tag = "Ф";
                        sButton.Tag = "Ы";
                        dButton.Tag = "В";
                        fButton.Tag = "А";
                        gButton.Tag = "П";
                        hButton.Tag = "Р";
                        jButton.Tag = "О";
                        kButton.Tag = "Л";
                        lButton.Tag = "Д";
                        zButton.Tag = "Я";
                        xButton.Tag = "Ч";
                        cButton.Tag = "С";
                        vButton.Tag = "М";
                        bButton.Tag = "И";
                        nButton.Tag = "Т";
                        mButton.Tag = "Ь";
                        addButton1.Tag = "Х";
                        addButton2.Tag = "Ъ";
                        addButton3.Tag = "Ж";
                        addButton4.Tag = "Э";
                        addButton5.Tag = "Б";
                        addButton6.Tag = "Ю";
                        //addButton19.Tag = ")";
                        //addButton20.Tag = "=";
                        //addButton7.Tag = "?";

                    }

                }
                else
                {
                    if(IsEngLanguage==true)
                    {
                        qButton.Tag = "q";
                        wButton.Tag = "w";
                        eButton.Tag = "e";
                        rButton.Tag = "r";
                        tButton.Tag = "t";
                        yButton.Tag = "y";
                        uButton.Tag = "u";
                        iButton.Tag = "i";
                        oButton.Tag = "o";
                        pButton.Tag = "p";
                        aButton.Tag = "a";
                        sButton.Tag = "s";
                        dButton.Tag = "d";
                        fButton.Tag = "f";
                        gButton.Tag = "g";
                        hButton.Tag = "h";
                        jButton.Tag = "j";
                        kButton.Tag = "k";
                        lButton.Tag = "l";
                        zButton.Tag = "z";
                        xButton.Tag = "x";
                        cButton.Tag = "c";
                        vButton.Tag = "v";
                        bButton.Tag = "b";
                        nButton.Tag = "n";
                        mButton.Tag = "m";
                        addButton1.Tag = "[";
                        addButton2.Tag = "]";
                        addButton3.Tag = ";";
                        addButton4.Tag = ":";
                        addButton5.Tag = ",";
                        addButton6.Tag = ".";
                        addButton7.Tag = "!";

                    }
                    else
                    {
                        qButton.Tag = "й";
                        wButton.Tag = "ц";
                        eButton.Tag = "у";
                        rButton.Tag = "к";
                        tButton.Tag = "е";
                        yButton.Tag = "н";
                        uButton.Tag = "г";
                        iButton.Tag = "ш";
                        oButton.Tag = "щ";
                        pButton.Tag = "з";
                        aButton.Tag = "ф";
                        sButton.Tag = "ы";
                        dButton.Tag = "в";
                        fButton.Tag = "а";
                        gButton.Tag = "п";
                        hButton.Tag = "р";
                        jButton.Tag = "о";
                        kButton.Tag = "л";
                        lButton.Tag = "д";
                        zButton.Tag = "я";
                        xButton.Tag = "ч";
                        cButton.Tag = "с";
                        vButton.Tag = "м";
                        bButton.Tag = "и";
                        nButton.Tag = "т";
                        mButton.Tag = "ь";
                        addButton1.Tag = "х";
                        addButton2.Tag = "ъ";
                        addButton3.Tag = "ж";
                        addButton4.Tag = "э";
                        addButton5.Tag = "б";
                        addButton6.Tag = "ю";
                       
                        //addButton19.Tag = ")";
                        //addButton20.Tag = "=";
                        //addButton7.Tag = "?";

                    }
                }
            }));

        private ICommand _switchLanguageCommand;
        private ICommand _sendKeysCommand;
        private ICommand _deleteCommand;
        private ICommand _closeKeyBoardCommand;
        private ICommand _enterCommand;
        private ICommand _symbolsCommand;


        public static readonly DependencyProperty IsSymbolsProperty = DependencyProperty.Register(
            "IsSymbols", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool IsSymbols
        {
            get { return (bool) GetValue(IsSymbolsProperty); }
            set { SetValue(IsSymbolsProperty, value); }
        }

        public ICommand CloseKeyBoardCommand =>
            _closeKeyBoardCommand ?? (_closeKeyBoardCommand = new Command(() => { IsOpen = false; }));

        public ICommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new Command(() => {  Send(Keys.Back, false); }));
        public ICommand EnterCommand =>
            _enterCommand ?? (_enterCommand = new Command(() =>
            {
                
                Send(Keys.Enter, false, false);
            }));


        public ICommand SymbolsCommand =>
            _symbolsCommand ?? (_symbolsCommand = new Command(() =>
            {
                IsSymbols = !IsSymbols;
                if (IsSymbols)
                {
                    addButton1.Visibility = Visibility.Visible;
                    addButton2.Visibility = Visibility.Visible;
                    addButton3.Visibility = Visibility.Visible;
                    addButton4.Visibility = Visibility.Visible;
                    addButton5.Visibility = Visibility.Visible;
                    addButton6.Visibility = Visibility.Visible;
                    //IsEngLanguage = false;
                    symbolButton.Tag = "АБВ";
                    langButton.Visibility = Visibility.Collapsed;
                    shiftButton.Visibility = Visibility.Collapsed;
                    qButton.Tag = "@";
                    wButton.Tag = "#";
                    eButton.Tag = "-";
                    rButton.Tag = "_";
                    tButton.Tag = "+";
                    yButton.Tag = "(";
                    uButton.Tag = ")";
                    iButton.Tag = "{";
                    oButton.Tag = "}";
                    pButton.Tag = "&";
                    aButton.Tag = "“";
                    sButton.Tag = "”";
                    dButton.Tag = "‘";
                    fButton.Tag = "=";
                    gButton.Tag = ":";
                    hButton.Tag = ";";
                    jButton.Tag = "!";
                    kButton.Tag = "?";
                    lButton.Tag = "$";
                    zButton.Tag = "<";
                    xButton.Tag = ">";
                    cButton.Tag = ".";
                    vButton.Tag = ",";
                    bButton.Tag = "*";
                    nButton.Tag = "|";
                    mButton.Tag = "~";
                    addButton1.Tag = "[";
                    addButton2.Tag = "]";
                    addButton3.Tag = "%";
                    addButton4.Tag = "№";
                    addButton5.Tag = "\\";
                    addButton6.Tag = "/";
                    backButton.Tag = "Удалить";
                }
                else
                {
                    
                    symbolButton.Tag = "?!@";

                    langButton.Visibility = Visibility.Visible;
                    shiftButton.Visibility = Visibility.Visible;
                    if (IsEngLanguage == true)
                    {
                        addButton3.Visibility = Visibility.Collapsed;
                        addButton4.Visibility = Visibility.Collapsed;
                        addButton1.Visibility = Visibility.Collapsed;
                        addButton2.Visibility = Visibility.Collapsed;
                        addButton5.Visibility = Visibility.Collapsed;
                        addButton6.Visibility = Visibility.Collapsed;
                        langButton.Tag = "RUS";
                        backButton.Tag = "Delete";
                        addButton1.Tag = "[";
                        addButton2.Tag = "]";
                        addButton3.Tag = ":";
                        addButton4.Tag = ";";
                        addButton5.Tag = ",";
                        addButton6.Tag = ".";

                        if (CapsPressed)
                        {
                            qButton.Tag = "Q";
                            wButton.Tag = "W";
                            eButton.Tag = "E";
                            rButton.Tag = "R";
                            tButton.Tag = "T";
                            yButton.Tag = "Y";
                            uButton.Tag = "U";
                            iButton.Tag = "I";
                            oButton.Tag = "O";
                            pButton.Tag = "P";
                            aButton.Tag = "A";
                            sButton.Tag = "S";
                            dButton.Tag = "D";
                            fButton.Tag = "F";
                            gButton.Tag = "G";
                            hButton.Tag = "H";
                            jButton.Tag = "J";
                            kButton.Tag = "K";
                            lButton.Tag = "L";
                            zButton.Tag = "Z";
                            xButton.Tag = "X";
                            cButton.Tag = "C";
                            vButton.Tag = "V";
                            bButton.Tag = "B";
                            nButton.Tag = "N";
                            mButton.Tag = "M";
                            addButton7.Tag = "!";

                        }
                        else
                        {
                            qButton.Tag = "q";
                            wButton.Tag = "w";
                            eButton.Tag = "e";
                            rButton.Tag = "r";
                            tButton.Tag = "t";
                            yButton.Tag = "y";
                            uButton.Tag = "u";
                            iButton.Tag = "i";
                            oButton.Tag = "o";
                            pButton.Tag = "p";
                            aButton.Tag = "a";
                            sButton.Tag = "s";
                            dButton.Tag = "d";
                            fButton.Tag = "f";
                            gButton.Tag = "g";
                            hButton.Tag = "h";
                            jButton.Tag = "j";
                            kButton.Tag = "k";
                            lButton.Tag = "l";
                            zButton.Tag = "z";
                            xButton.Tag = "x";
                            cButton.Tag = "c";
                            vButton.Tag = "v";
                            bButton.Tag = "b";
                            nButton.Tag = "n";
                            mButton.Tag = "m";
                            addButton1.Tag = "[";
                            addButton2.Tag = "]";
                            addButton3.Tag = ";";
                            addButton4.Tag = ":";
                            addButton5.Tag = ",";
                            addButton6.Tag = ".";
                            addButton7.Tag = "!";

                        }
                    }
                    else
                    {
                        langButton.Tag = "ENG";
                        backButton.Tag = "Удалить";


                        if (CapsPressed)
                        {
                            qButton.Tag = "Й";
                            wButton.Tag = "Ц";
                            eButton.Tag = "У";
                            rButton.Tag = "К";
                            tButton.Tag = "Е";
                            yButton.Tag = "Н";
                            uButton.Tag = "Г";
                            iButton.Tag = "Ш";
                            oButton.Tag = "Щ";
                            pButton.Tag = "З";
                            aButton.Tag = "Ф";
                            sButton.Tag = "Ы";
                            dButton.Tag = "В";
                            fButton.Tag = "А";
                            gButton.Tag = "П";
                            hButton.Tag = "Р";
                            jButton.Tag = "О";
                            kButton.Tag = "Л";
                            lButton.Tag = "Д";
                            zButton.Tag = "Я";
                            xButton.Tag = "Ч";
                            cButton.Tag = "С";
                            vButton.Tag = "М";
                            bButton.Tag = "И";
                            nButton.Tag = "Т";
                            mButton.Tag = "Ь";
                            addButton1.Tag = "Х";
                            addButton2.Tag = "Ъ";
                            addButton3.Tag = "Ж";
                            addButton4.Tag = "Э";
                            addButton5.Tag = "Б";
                            addButton6.Tag = "Ю";
                            //addButton19.Tag = ")";
                            //addButton20.Tag = "=";
                            //addButton7.Tag = "?";

                        }
                        else
                        {
                            qButton.Tag = "й";
                            wButton.Tag = "ц";
                            eButton.Tag = "у";
                            rButton.Tag = "к";
                            tButton.Tag = "е";
                            yButton.Tag = "н";
                            uButton.Tag = "г";
                            iButton.Tag = "ш";
                            oButton.Tag = "щ";
                            pButton.Tag = "з";
                            aButton.Tag = "ф";
                            sButton.Tag = "ы";
                            dButton.Tag = "в";
                            fButton.Tag = "а";
                            gButton.Tag = "п";
                            hButton.Tag = "р";
                            jButton.Tag = "о";
                            kButton.Tag = "л";
                            lButton.Tag = "д";
                            zButton.Tag = "я";
                            xButton.Tag = "ч";
                            cButton.Tag = "с";
                            vButton.Tag = "м";
                            bButton.Tag = "и";
                            nButton.Tag = "т";
                            mButton.Tag = "ь";
                            addButton1.Tag = "х";
                            addButton2.Tag = "ъ";
                            addButton3.Tag = "ж";
                            addButton4.Tag = "э";
                            addButton5.Tag = "б";
                            addButton6.Tag = "ю";
                            
                            //addButton19.Tag = ")";
                            //addButton20.Tag = "=";
                            //addButton7.Tag = "?";

                        }
                    }
                }
            }));

        public ICommand SwitchLanguageCommand => _switchLanguageCommand ?? (_switchLanguageCommand = new Command(() =>
        {
            IsEngLanguage = !IsEngLanguage;
            //TODO смена языка через тэги
            if (IsEngLanguage == true)
            {
                addButton1.Visibility = Visibility.Collapsed;
                addButton2.Visibility = Visibility.Collapsed;
                addButton3.Visibility = Visibility.Collapsed;
                addButton4.Visibility = Visibility.Collapsed;
                addButton5.Visibility = Visibility.Collapsed;
                addButton6.Visibility = Visibility.Collapsed;
                langButton.Tag = "RUS";
                backButton.Tag = "Delete";
                addButton1.Tag = "[";
                addButton2.Tag = "]";
                addButton3.Tag = ":";
                addButton4.Tag = ";";
                addButton5.Tag = ",";
                addButton6.Tag = ".";

                if (CapsPressed)
                {
                    qButton.Tag = "Q";
                    wButton.Tag = "W";
                    eButton.Tag = "E";
                    rButton.Tag = "R";
                    tButton.Tag = "T";
                    yButton.Tag = "Y";
                    uButton.Tag = "U";
                    iButton.Tag = "I";
                    oButton.Tag = "O";
                    pButton.Tag = "P";
                    aButton.Tag = "A";
                    sButton.Tag = "S";
                    dButton.Tag = "D";
                    fButton.Tag = "F";
                    gButton.Tag = "G";
                    hButton.Tag = "H";
                    jButton.Tag = "J";
                    kButton.Tag = "K";
                    lButton.Tag = "L";
                    zButton.Tag = "Z";
                    xButton.Tag = "X";
                    cButton.Tag = "C";
                    vButton.Tag = "V";
                    bButton.Tag = "B";
                    nButton.Tag = "N";
                    mButton.Tag = "M";
                    addButton7.Tag = "!";

                }
                else
                {
                    qButton.Tag = "q";
                    wButton.Tag = "w";
                    eButton.Tag = "e";
                    rButton.Tag = "r";
                    tButton.Tag = "t";
                    yButton.Tag = "y";
                    uButton.Tag = "u";
                    iButton.Tag = "i";
                    oButton.Tag = "o";
                    pButton.Tag = "p";
                    aButton.Tag = "a";
                    sButton.Tag = "s";
                    dButton.Tag = "d";
                    fButton.Tag = "f";
                    gButton.Tag = "g";
                    hButton.Tag = "h";
                    jButton.Tag = "j";
                    kButton.Tag = "k";
                    lButton.Tag = "l";
                    zButton.Tag = "z";
                    xButton.Tag = "x";
                    cButton.Tag = "c";
                    vButton.Tag = "v";
                    bButton.Tag = "b";
                    nButton.Tag = "n";
                    mButton.Tag = "m";
                    addButton1.Tag = "[";
                    addButton2.Tag = "]";
                    addButton3.Tag = ";";
                    addButton4.Tag = ":";
                    addButton5.Tag = ",";
                    addButton6.Tag = ".";
                    addButton7.Tag = "!";

                }
            }
            else
            {
                addButton1.Visibility = Visibility.Visible;
                addButton2.Visibility = Visibility.Visible;
                addButton3.Visibility = Visibility.Visible;
                addButton4.Visibility = Visibility.Visible;
                addButton5.Visibility = Visibility.Visible;
                addButton6.Visibility = Visibility.Visible;
                langButton.Tag = "ENG";
                backButton.Tag = "Удалить";


                if (CapsPressed)
                {
                    qButton.Tag = "Й";
                    wButton.Tag = "Ц";
                    eButton.Tag = "У";
                    rButton.Tag = "К";
                    tButton.Tag = "Е";
                    yButton.Tag = "Н";
                    uButton.Tag = "Г";
                    iButton.Tag = "Ш";
                    oButton.Tag = "Щ";
                    pButton.Tag = "З";
                    aButton.Tag = "Ф";
                    sButton.Tag = "Ы";
                    dButton.Tag = "В";
                    fButton.Tag = "А";
                    gButton.Tag = "П";
                    hButton.Tag = "Р";
                    jButton.Tag = "О";
                    kButton.Tag = "Л";
                    lButton.Tag = "Д";
                    zButton.Tag = "Я";
                    xButton.Tag = "Ч";
                    cButton.Tag = "С";
                    vButton.Tag = "М";
                    bButton.Tag = "И";
                    nButton.Tag = "Т";
                    mButton.Tag = "Ь";
                    addButton1.Tag = "Х";
                    addButton2.Tag = "Ъ";
                    addButton3.Tag = "Ж";
                    addButton4.Tag = "Э";
                    addButton5.Tag = "Б";
                    addButton6.Tag = "Ю";
                    //addButton19.Tag = ")";
                    //addButton20.Tag = "=";
                    //addButton7.Tag = "?";

                }
                else
                {
                    qButton.Tag = "й";
                    wButton.Tag = "ц";
                    eButton.Tag = "у";
                    rButton.Tag = "к";
                    tButton.Tag = "е";
                    yButton.Tag = "н";
                    uButton.Tag = "г";
                    iButton.Tag = "ш";
                    oButton.Tag = "щ";
                    pButton.Tag = "з";
                    aButton.Tag = "ф";
                    sButton.Tag = "ы";
                    dButton.Tag = "в";
                    fButton.Tag = "а";
                    gButton.Tag = "п";
                    hButton.Tag = "р";
                    jButton.Tag = "о";
                    kButton.Tag = "л";
                    lButton.Tag = "д";
                    zButton.Tag = "я";
                    xButton.Tag = "ч";
                    cButton.Tag = "с";
                    vButton.Tag = "м";
                    bButton.Tag = "и";
                    nButton.Tag = "т";
                    mButton.Tag = "ь";
                    addButton1.Tag = "х";
                    addButton2.Tag = "ъ";
                    addButton3.Tag = "ж";
                    addButton4.Tag = "э";
                    addButton5.Tag = "б";
                    addButton6.Tag = "ю";
                    //addButton19.Tag = ")";
                    //addButton20.Tag = "=";
                    //addButton7.Tag = "?";

                }
            }

            //try
            //{
            //    ChoosedCulture = ChoosedCulture == CultureInfo.GetCultureInfo("en-US")
            //        ? CultureInfo.GetCultureInfo("ru-RU")
            //        : CultureInfo.GetCultureInfo("en-US");
            //}
            //catch (Exception ee)
            //{
            //    System.Windows.MessageBox.Show(ee.Message);
            //}
        }));

        public ICommand SendKeysCommand => _sendKeysCommand ?? (_sendKeysCommand = new Command(a =>
        {
            try
            {
               // string c = a.ToString().ToLower();
                if (a is string key)
                {
                    
                    SendString(key);
                   // Send((Keys)49, false, true);
                   // Send(Keys.Back, false);
                   
                    //switch (key)
                    //{
                    //    case "ә":
                    //        SendString("ә");
                    //        Send((Keys)49, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ә":
                    //        SendString("Ә");
                    //        Send((Keys)49, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "і":
                    //        SendString("і");
                    //        Send((Keys)50, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "І":
                    //        SendString("І");
                    //        Send((Keys)50, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ң":
                    //        SendString("ң");
                    //        Send((Keys)51, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ң":
                    //        SendString("Ң");
                    //        Send((Keys)51, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ғ":
                    //        SendString("ғ");
                    //        Send((Keys)52, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ғ":
                    //        SendString("Ғ");
                    //        Send((Keys)52, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ү":
                    //        SendString("ү");
                    //        Send((Keys)53, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ү":
                    //        SendString("Ү");
                    //        Send((Keys)53, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ұ":
                    //        SendString("ұ");
                    //        Send((Keys)54, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ұ":
                    //        SendString("Ұ");
                    //        Send((Keys)54, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "қ":
                    //        SendString("қ");
                    //        Send((Keys)55, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Қ":
                    //        SendString("Қ");
                    //        Send((Keys)55, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ө":
                    //        SendString("ө");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ө":
                    //        SendString("Ө");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "һ":
                    //        SendString("һ");
                    //        Send((Keys)57, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Һ":
                    //        SendString("Һ");
                    //        Send((Keys)57, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "q":
                    //        SendString("q");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;


                    //    case "Q":
                    //        SendString("Q");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "w":
                    //        SendString("w");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "W":
                    //        SendString("W");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "e":
                    //        SendString("e");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "E":
                    //        SendString("E");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "r":
                    //        SendString("r");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "R":
                    //        SendString("R");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "t":
                    //        SendString("t");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "T":
                    //        SendString("T");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "y":
                    //        SendString("y");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Y":
                    //        SendString("Y");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "u":
                    //        SendString("u");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "U":
                    //        SendString("U");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "i":
                    //        SendString("i");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "I":
                    //        SendString("I");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "o":
                    //        SendString("o");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "O":
                    //        SendString("O");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "p":
                    //        SendString("p");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "P":
                    //        SendString("P");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "a":
                    //        SendString("a");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "A":
                    //        SendString("A");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "s":
                    //        SendString("s");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "S":
                    //        SendString("S");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "d":
                    //        SendString("d");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "D":
                    //        SendString("D");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "f":
                    //        SendString("f");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "F":
                    //        SendString("F");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "g":
                    //        SendString("g");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "G":
                    //        SendString("G");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "h":
                    //        SendString("h");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "H":
                    //        SendString("H");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "j":
                    //        SendString("j");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "J":
                    //        SendString("J");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "k":
                    //        SendString("k");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "K":
                    //        SendString("K");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "l":
                    //        SendString("l");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "L":
                    //        SendString("L");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "z":
                    //        SendString("z");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Z":
                    //        SendString("Z");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "x":
                    //        SendString("x");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "X":
                    //        SendString("X");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "c":
                    //        SendString("");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "C":
                    //        SendString("C");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "v":
                    //        SendString("v");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "V":
                    //        SendString("V");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "b":
                    //        SendString("b");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "B":
                    //        SendString("B");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "n":
                    //        SendString("n");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "N":
                    //        SendString("N");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "m":
                    //        SendString("m");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "M":
                    //        SendString("M");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "1":
                    //        SendString("1");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "2":
                    //        SendString("2");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "3":
                    //        SendString("3");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "4":
                    //        SendString("4");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "5":
                    //        SendString("5");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "6":
                    //        SendString("6");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "7":
                    //        SendString("7");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "8":
                    //        SendString("8");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "9":
                    //        SendString("9");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "0":
                    //        SendString("0");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "-":
                    //        SendString("-");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "_":
                    //        SendString("_");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "=":
                    //        SendString("=");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "+":
                    //        SendString("+");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case ":":
                    //        SendString(":");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case ";":
                    //        SendString(";");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "@":
                    //        SendString("@");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "!":
                    //        SendString("!");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "?":
                    //        SendString("?");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case ")":
                    //        SendString(")");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "(":
                    //        SendString("(");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "[":
                    //        SendString("[");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "]":
                    //        SendString("]");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case ".":
                    //        SendString(".");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case ",":
                    //        SendString(",");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "`":
                    //        SendString("`");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ё":
                    //        SendString("ё");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ё":
                    //        SendString("Ё");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "й":
                    //        SendString("й");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Й":
                    //        SendString("Й");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "ц":
                    //        SendString("ц");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "Ц":
                    //        SendString("Ц");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "у":
                    //        SendString("у");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;
                    //    case "У":
                    //        SendString("У");
                    //        Send((Keys)56, false, true);
                    //        Send(Keys.Back, false);
                    //        return;

                    //}

                    //var Key = Enum.Parse(typeof(Keys),
                    //    AllWord.Keys.Contains(key) ? AllWord[key].ToUpper() : key.ToUpper());
                    //Send((Keys)Key, false);
                    //InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(ChoosedCulture);
                }
            }
            catch
            {
                //
            }
        }));

        public static void SendString(string s)
        {
            // Construct list of inputs in order to send them through a single SendInput call at the end.
            List<INPUT> inputs = new List<INPUT>();

            // Loop through each Unicode character in the string.
            foreach (char c in s)
            {
                // First send a key down, then a key up.
                foreach (bool keyUp in new bool[] { false, true })
                {
                    // INPUT is a multi-purpose structure which can be used 
                    // for synthesizing keystrokes, mouse motions, and button clicks.
                    INPUT input = new INPUT
                    {
                        // Need a keyboard event.
                        type = INPUT_KEYBOARD,
                        u = new InputUnion
                        {
                            // KEYBDINPUT will contain all the information for a single keyboard event
                            // (more precisely, for a single key-down or key-up).
                            ki = new KEYBDINPUT
                            {
                                // Virtual-key code must be 0 since we are sending Unicode characters.
                                wVk = 0,

                                // The Unicode character to be sent.
                                wScan = c,

                                // Indicate that we are sending a Unicode character.
                                // Also indicate key-up on the second iteration.
                                dwFlags = KEYEVENTF_UNICODE | (keyUp ? KEYEVENTF_KEYUP : 0),

                                dwExtraInfo = GetMessageExtraInfo(),
                            }
                        }
                    };

                    // Add to the list (to be sent later).
                    inputs.Add(input);
                }
            }

            // Send all inputs together using a Windows API call.
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            /*Virtual Key code.  Must be from 1-254.  If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.*/
            public ushort wVk;
            /*A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.*/
            public ushort wScan;
            /*Specifies various aspects of a keystroke.  See the KEYEVENTF_ constants for more information.*/
            public uint dwFlags;
            /*The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.*/
            public uint time;
            /*An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.*/
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private static void PressKey(Keys key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
            {
                keybd_event((byte) key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr) 0);
            }
            else
            {
                keybd_event((byte) key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr) 0);
            }
        }
        
        void Send(Keys bukva, bool shift, bool cifra = false)
        {

            // FocusWB.Invoke(null, null);
            //App.OnUserEvent();

            //if (App.WebBrow != null)
            //{
            //    App.CurrentApp.MainWindow.Activate();
            //    App.CurrentApp.MainWindow.Focus();
            //    App.WebBrow.Focus();
            //}


            if ((shift || ShiftPressed || CapsPressed) && !cifra )
                PressKey(Keys.LShiftKey, false);

            
            PressKey(bukva, false);
            PressKey(bukva, true);
            if ((shift || ShiftPressed || CapsPressed)&& !cifra)
                PressKey(Keys.LShiftKey, true);
            ShiftPressed = false;
        }

        private readonly Dictionary<string, string> AllWord = new Dictionary<string, string>()
        {
            {"й", "q"},
            {"ц", "w"},
            {"у", "e"},
            {"к", "r"},
            {"е", "t"},
            {"н", "y"},
            {"г", "u"},
            {"ш", "i"},
            {"щ", "o"},
            {"з", "p"},
            {"ф", "a"},
            {"ы", "s"},
            {"в", "d"},
            {"а", "f"},
            {"п", "g"},
            {"р", "h"},
            {"о", "j"},
            {"л", "k"},
            {"д", "l"},
            {"я", "z"},
            {"ч", "x"},
            {"с", "c"},
            {"м", "v"},
            {"и", "b"},
            {"т", "n"},
            {"ь", "m"},
            {"ю", "?"},
            {"!", "!"}
        };
    }
}
