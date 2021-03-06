﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Utility.WindowsForm;
using Utility.存储;
using Utility.通用;
using 通用访问;
using 通用访问.DTO;

namespace 通用访问工具
{
    public partial class F设备 : Form
    {
        private M设备 _当前设备;

        private Point _鼠标位置;

        private IPEndPoint _地址;

        private string _名称;

        public F设备(IPEndPoint __地址, string __名称 = "")
        {
            _地址 = __地址;
            _名称 = __名称;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.out标题.Text += " " + Assembly.GetExecutingAssembly().GetName().Version;
            this.do断开设备.Enabled = false;
            this.do工程.Enabled = false;
            this.do开发.Enabled = false;

            this.do对象_刷新.Click += do对象_刷新_Click;
            this.out对象菜单.Opening += out对象菜单_Opening;
            this.do断开设备.Click += do断开设备_Click;

            this.do工程.Click += do工程_Click;
            this.do开发.Click += do开发_Click;
            this.do客户.Click += do客户_Click;

            加载设备(_地址, _名称);
        }

        void do开发_Click(object sender, EventArgs e)
        {
            if (_当前设备 != null && _当前设备.访问入口 != null)
            {
                this.out对象列表容器.激活控件(_当前设备.开发对象列表控件);
                this.do开发.Enabled = false;
                this.do工程.Enabled = true;
                this.do客户.Enabled = true;
                this.do开发.BackColor = Color.Yellow;
                this.do工程.BackColor = Color.FromArgb(38, 164, 221);
                this.do客户.BackColor = Color.FromArgb(38, 164, 221);
                _当前设备.视图 = E角色.开发;
                更新角色();
            }
        }

        void do工程_Click(object sender, EventArgs e)
        {
            if (_当前设备 != null && _当前设备.访问入口 != null)
            {
                this.out对象列表容器.激活控件(_当前设备.工程对象列表控件);
                this.do客户.Enabled = true;
                this.do开发.Enabled = true;
                this.do工程.Enabled = false;
                this.do开发.BackColor = Color.FromArgb(38, 164, 221);
                this.do客户.BackColor = Color.FromArgb(38, 164, 221);
                this.do工程.BackColor = Color.Yellow;
                _当前设备.视图 = E角色.工程;
                更新角色();
            }
        }

        void do客户_Click(object sender, EventArgs e)
        {
            if (_当前设备 != null && _当前设备.访问入口 != null)
            {
                this.out对象列表容器.激活控件(_当前设备.客户对象列表控件);
                this.do客户.Enabled = false;
                this.do开发.Enabled = true;
                this.do工程.Enabled = true;
                this.do工程.BackColor = Color.FromArgb(38, 164, 221);
                this.do开发.BackColor = Color.FromArgb(38, 164, 221);
                this.do客户.BackColor = Color.Yellow;
                _当前设备.视图 = E角色.客户;
                更新角色();
            }
        }

        private void 更新角色()
        {
            foreach (var __kv in _当前设备.对象明细控件.所有标签)
            {
                var __控件 = __kv.Value as F对象;
                if (__控件 != null)
                {
                    __控件.更新角色(_当前设备.视图);
                }
            }
        }

        void do断开设备_Click(object sender, EventArgs e)
        {
            if (_当前设备 != null && _当前设备.访问入口 != null)
            {
                _当前设备.访问入口.断开();
                _当前设备.工程对象列表控件.Nodes.Clear();
                _当前设备.开发对象列表控件.Nodes.Clear();
                _当前设备.客户对象列表控件.Nodes.Clear();
                _当前设备.对象明细控件.所有标签.Keys.ToList().ForEach(q =>
                {
                    if (q != "通知")
                    {
                        _当前设备.对象明细控件.删除(q);
                    }
                });
                this.out提示.Visible = true;
            }
        }

        void TV_MouseDown(object sender, MouseEventArgs e)
        {
            _鼠标位置 = e.Location;
        }

        void out对象菜单_Opening(object sender, CancelEventArgs e)
        {
            var __菜单 = sender as ContextMenuStrip;
            if (__菜单 == null) return;
            var __tv = __菜单.SourceControl as TreeView;
            if (__tv == null) return;

            var __node = __tv.GetNodeAt(_鼠标位置);
            if (__node == null || __node.Tag == null)
            {
                e.Cancel = true;
                return;
            }
            __tv.SelectedNode = __node;
        }

        void do对象_刷新_Click(object sender, EventArgs e)
        {
            var __tv = out对象菜单.SourceControl as TreeView;
            var __node = __tv.GetNodeAt(_鼠标位置);
            if (__node == null || __node.Tag == null)
            {
                return;
            }
            var __对象 = __node.Tag as M对象概要;
            if (_当前设备.对象明细控件.所有标签.ContainsKey(__对象.名称))
            {
                _当前设备.对象明细控件.删除(__对象.名称);
            }
            var __对象明细 = _当前设备.访问入口.查询对象明细(__对象.名称);
            _当前设备.对象明细控件.添加(__对象.名称, new F对象(__对象.名称, __对象明细, _当前设备.访问入口, _当前设备.视图) { Dock = DockStyle.Fill });
        }

        void out对象列表_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //查询对象明细
            var __对象 = e.Node.Tag as M对象概要;
            if (__对象 == null)
            {
                return;
            }
            var __对象明细 = _当前设备.访问入口.查询对象明细(__对象.名称);
            if (_当前设备.对象明细控件.所有标签.ContainsKey(__对象.名称))
            {
                _当前设备.对象明细控件.激活(__对象.名称);
            }
            else
            {
                _当前设备.对象明细控件.添加(__对象.名称, new F对象(__对象.名称, __对象明细, _当前设备.访问入口, _当前设备.视图) { Dock = DockStyle.Fill });
            }
        }

        void 加载设备(IPEndPoint __地址, string __名称 = "")
        {
            //加载对象
            var __设备 = new M设备 { IP = __地址.Address, 端口号 = __地址.Port, 名称 = __名称 };
            this.out提示.Visible = false;
            this.do断开设备.Enabled = true;
            Action __显示连接异常 = () => this.BeginInvoke(new Action(() =>
            {
                __设备.工程对象列表控件.ForeColor = Color.Red;
                __设备.开发对象列表控件.ForeColor = Color.Red;
                __设备.客户对象列表控件.ForeColor = Color.Red;
            }));
            Action __显示连接正常 = () => this.BeginInvoke(new Action(() =>
            {
                __设备.工程对象列表控件.ForeColor = Color.Black;
                __设备.开发对象列表控件.ForeColor = Color.Black;
                __设备.客户对象列表控件.ForeColor = Color.Black;
            }));
            _当前设备 = __设备;
            if (__设备.访问入口 == null)
            {
                __设备.工程对象列表控件 = new TreeView { BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.None, Dock = DockStyle.Fill, ShowNodeToolTips = true, ContextMenuStrip = this.out对象菜单 };
                __设备.工程对象列表控件.NodeMouseDoubleClick += out对象列表_NodeMouseDoubleClick;
                __设备.工程对象列表控件.MouseDown += TV_MouseDown;

                __设备.开发对象列表控件 = new TreeView { BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.None, Dock = DockStyle.Fill, ShowNodeToolTips = true, ContextMenuStrip = this.out对象菜单 };
                __设备.开发对象列表控件.NodeMouseDoubleClick += out对象列表_NodeMouseDoubleClick;
                __设备.开发对象列表控件.MouseDown += TV_MouseDown;

                __设备.客户对象列表控件 = new TreeView { BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.None, Dock = DockStyle.Fill, ShowNodeToolTips = true, ContextMenuStrip = this.out对象菜单 };
                __设备.客户对象列表控件.NodeMouseDoubleClick += out对象列表_NodeMouseDoubleClick;
                __设备.客户对象列表控件.MouseDown += TV_MouseDown;

                this.out对象列表容器.添加控件(__设备.工程对象列表控件);
                this.out对象列表容器.添加控件(__设备.开发对象列表控件);
                this.out对象列表容器.添加控件(__设备.客户对象列表控件);
                this.out对象列表容器.激活控件(__设备.工程对象列表控件);

                __设备.对象明细控件 = new UTab() { Dock = DockStyle.Fill };
                this.out对象明细容器.添加控件(__设备.对象明细控件);
                this.out对象明细容器.激活控件(__设备.对象明细控件);
                var __通知控件 = new F通知() { Dock = DockStyle.Fill };
                __设备.对象明细控件.添加("通知", __通知控件);
                
                __设备.访问入口 = FT通用访问工厂.创建客户端();
                __设备.访问入口.收到了通知 += __通知控件.处理通知;
                __设备.访问入口.收到了事件 += q => __通知控件.处理通知(new M通知 { 对象 = q.对象名称, 角色= E角色.所有, 重要性 = "普通", 概要 = q.事件名称, 详细 = q.实参列表 == null ? "" : string.Join(",", q.实参列表.Select(k => string.Format("{0}:{1}", k.名称, k.值))) });
                __设备.访问入口.已断开 += __主动 =>
                {
                    if (!__主动)
                    {
                        __显示连接异常();
                    }
                };
                __设备.访问入口.已连接 += __显示连接正常;
                初始化对象列表(__设备);
                __显示连接正常();
                //this.do工程.PerformClick();
                this.do客户_Click(this.do工程, EventArgs.Empty);
            }
            else
            {
                this.out对象明细容器.激活控件(__设备.对象明细控件);
                if (!__设备.访问入口.连接正常)
                {
                    初始化对象列表(__设备);
                    __显示连接正常();
                    if (__设备.视图 == E角色.工程)
                    {
                        this.do工程_Click(this.do工程, EventArgs.Empty);
                    }
                    if (__设备.视图 == E角色.开发)
                    {
                        this.do开发_Click(this.do开发, EventArgs.Empty);
                    }
                    if (__设备.视图 == E角色.客户)
                    {
                        this.do客户_Click(this.do开发, EventArgs.Empty);
                    }
                }
            }
        }

        private void 初始化对象列表(M设备 __设备)
        {
            __设备.工程对象列表控件.Nodes.Clear();
            __设备.开发对象列表控件.Nodes.Clear();
            __设备.客户对象列表控件.Nodes.Clear();
            __设备.访问入口.连接(new IPEndPoint(__设备.IP, __设备.端口号));
            if (!Equals(__设备.IP, IPAddress.Loopback))
            {
                Thread.Sleep(1000); //当前因为LINUX设备端有延时? 需要停顿一下才能发送
            }
            var __对象列表 = __设备.访问入口.查询可访问对象();
            设置对象列表控件(__设备.工程对象列表控件, __对象列表, E角色.工程);
            设置对象列表控件(__设备.开发对象列表控件, __对象列表, E角色.开发);
            设置对象列表控件(__设备.客户对象列表控件, __对象列表, E角色.客户);
        }

        private void 设置对象列表控件(TreeView __控件, M对象列表 __对象列表, E角色 __角色)
        {
            var __分类节点 = new Dictionary<string, TreeNode>();
            var __数量 = 0;
            __对象列表.对象列表.ForEach(q =>
            {
                if ((q.角色 & __角色) == __角色)
                {
                    __数量++;
                    TreeNode __node;
                    if (string.IsNullOrEmpty(q.分类))
                    {
                        __node = __控件.Nodes.Add(q.名称);
                    }
                    else
                    {
                        if (!__分类节点.ContainsKey(q.分类))
                        {
                            __分类节点[q.分类] = __控件.Nodes.Add(q.分类);
                        }
                        __node = __分类节点[q.分类].Nodes.Add(q.名称);
                    }
                    __node.Tag = q;
                }
            });
            if (__数量 < 30)
            {
                __控件.ExpandAll();
            }
        }

        private class M设备
        {
            public string 名称 { get; set; }
            public IPAddress IP { get; set; }
            public int 端口号 { get; set; }
            public TreeView 工程对象列表控件 { get; set; }
            public TreeView 开发对象列表控件 { get; set; }
            public TreeView 客户对象列表控件 { get; set; }
            public UTab 对象明细控件 { get; set; }
            public IT客户端 访问入口 { get; set; }
            public E角色 视图 { get; set; }
        }
    }
}
