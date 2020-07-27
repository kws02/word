using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DataContext : DbContext
    {
        public DbSet<복습> 복습 { get; set; }

        public DbSet<계정> 계정 { get; set; }

        public DbSet<로그> 로그 { get; set; }

        public DbSet<전산회계운용사_문제> 전산회계운용사_문제 { get; set; }

        public DbSet<전산회계운용사_이미지> 전산회계운용사_이미지 { get; set; }

        public DbSet<전산회계운용사_선택> 전산회계운용사_선택 { get; set; }

        public DbSet<전산회계운용사_답안> 전산회계운용사_답안 { get; set; }

        public DbSet<썸네일> 썸네일 { get; set; }
    }
}