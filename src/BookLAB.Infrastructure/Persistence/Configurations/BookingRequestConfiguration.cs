using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
{
    public void Configure(EntityTypeBuilder<BookingRequest> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("BookingRequests");
        builder.HasKey(br => br.Id);

        // 2. Cấu hình các thuộc tính cơ bản
        builder.Property(br => br.ResponseContext)
            .HasMaxLength(1000); // Ghi chú lý do từ chối hoặc hướng dẫn thêm

        builder.Property(br => br.BookingRequestStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // 3. Cấu hình các thuộc tính Auditing
        builder.Property(br => br.CreatedAt).IsRequired();
        builder.Property(br => br.CreatedBy).IsRequired();

        // 4. Cấu hình Quan hệ (Relationships)

        // Quan hệ với Booking (N-1)
        builder.HasOne(br => br.Booking)
            .WithMany() // Một Booking có thể có nhiều lượt Request (nếu bị từ chối rồi đặt lại)
            .HasForeignKey(br => br.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ với người gửi yêu cầu (RequestedByUserId)
        builder.HasOne(br => br.Requester)
            .WithMany()
            .HasForeignKey(br => br.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quan hệ với người phản hồi (ResponsedByUserId) - Cho phép Null vì lúc mới tạo chưa có người duyệt
        builder.HasOne(br => br.Reponser)
            .WithMany()
            .HasForeignKey(br => br.ResponsedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // 5. Cấu hình Index để truy vấn nhanh cho Dashboard của Manager

        // Tìm nhanh các đơn đang chờ duyệt (Pending)
        builder.HasIndex(br => br.BookingRequestStatus);

        // Tìm nhanh các đơn mà một Manager cụ thể đã xử lý
        builder.HasIndex(br => br.ResponsedByUserId)
            .HasFilter("\"ResponsedByUserId\" IS NOT NULL"); // Filtered Index để tối ưu dung lượng

        // Tìm lịch sử yêu cầu của một Giảng viên
        builder.HasIndex(br => br.RequestedByUserId);
    }
}