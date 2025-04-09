using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Category_Requests",
                columns: table => new
                {
                    category_request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requester_id = table.Column<int>(type: "int", nullable: false),
                    is_approved = table.Column<int>(type: "int", nullable: false),
                    is_viewed_by_admin = table.Column<bool>(type: "bit", nullable: false),
                    is_viewed_by_owner = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted_by_admin = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted_by_owner = table.Column<bool>(type: "bit", nullable: false),
                    rejection_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category_Requests", x => x.category_request_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    order_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    total_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order_Items",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Items", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_Order_Items_Orders_order_id",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Items_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "category_id", "name" },
                values: new object[,]
                {
                    { 1, "Văn học" },
                    { 2, "Khoa học" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "user_id", "address", "email", "image_url", "name", "password", "phone", "role", "username" },
                values: new object[,]
                {
                    { 1, "Hà Nội", "admin@gmail.com", "", "NVAdmin", "161ebd7d45089b3446ee4e0d86dbcf92", "0123456789", "Admin", "admin" },
                    { 2, "Hải Phòng", "test1@gmail.com", "", "NVOwner", "161ebd7d45089b3446ee4e0d86dbcf92", "0123456789", "Store Owner", "owner" },
                    { 3, "Cần Thơ", "test2@gmail.com", "", "NVCustomer", "161ebd7d45089b3446ee4e0d86dbcf92", "0123456789", "Customer", "owner" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "product_id", "author", "category_id", "description", "image_url", "name", "price", "quantity", "user_id" },
                values: new object[,]
                {
                    { 1, "Mã Thiện Đồng ", 1, "<p>Cuốn sách viết về một người chiến sĩ do Đảng Lao động và Nhà nước Việt Nam Dân chủ Cộng hòa sinh ra. Một anh bộ đội Cụ Hồ bước ra từ cuộc chiến tranh gian khổ và anh dũng.</p><p>Sau chiến tranh, nền kinh tế nước ta gần như kiệt quệ, càng khó khăn khi chiến tranh bảo vệ Tổ quốc lại nối tiếp xảy ra ở biên giới phía Tây Nam, phía Bắc; bị phong tỏa cấm vận kinh tế kéo dài mấy chục năm trời, khó khăn chồng chất khó khăn càng khiến cho cuộc sống của người dân gian nan, thiếu thốn. Tác giả muốn dựng lại những mảng hiện thực về một thời quá độ đáng ghi nhớ; cuốn sách được viết bằng tình cảm trân trọng và đồng cảm với những con người giàu tình yêu quê hương đất nước, được Bác Hồ là niềm tin thắp sáng dẫn dắt họ đi suốt những chặng đường dài.</p<p>Trong cuốn sách này là hình ảnh một con người tốt đẹp, sống tử tế, nhưng cái giá để làm người tử tế đắt lắm! Đó chính là việc làm và những thử thách mà anh phải trải qua trong suốt cuộc đời mình. Chính từ cuộc chiến đấu anh dũng, từ cuộc sống lao động xây dựng đất nước ở những năm tháng khó khăn, và bản chất tốt đẹp của người nông dân Việt Nam đã hình thành con người như Nguyễn Văn Dụ.</p>", "/imgProduct/book1.jpg", "NIỀM TIN THẮP SÁNG (TRUYỆN)", 53000m, 100, 2 },
                    { 2, "Lê Cung - Lê Thành Nam", 1, "<p>Nhìn lại lịch sử dân tộc và lịch sử Phật giáo kể từ khi được truyền vào nước ta, điều khẳng định là trên cả ba bình diện dựng nước, giữ nước và mở nước cũng như trong đấu tranh chống bất công và cường quyền, Tăng Ni và Phật Tử Việt Nam đã khẳng định mình là một bộ phận gắn bó chặt chẽ với dân tộc, đã góp tiếng nói và công sức vào cuộc đấu tranh vì Độc lập, Tự do của Tổ quốc, vì cuộc sống và hạnh phúc của nhân dân. </p><p>Cuộc vận động của Phật giáo Việt Nam năm 1963 thêm một lần nữa đã chứng minh điều đó.</p><p>Phật   giáo  đã có bề dày lịch sử gần hai chục thế kỷ. Trong quá trình đó  Phật   giáo  Việt Nam đã xây dựng cho mình truyền thống yêu nước, gắn bó dân tộc, góp phần quan trọng trong việc xây dựng nền văn hóa dân tộc, trong tư tưởng, đạo đức, tâm lý lối sống của nhân dân. Dù ở trong bất kỳ hoàn cảnh lịch sử nào,  Phật   giáo  Việt Nam chưa bao giờ tự tách mình khỏi khối cộng đồng dân tộc. Trong lầm than nô lệ,  Phật   giáo  Việt Nam đã cùng với dân tộc vùng lên đánh đổ ách thống trị ngoại bang. Trong sự hưng thịnh của đất nước Việt Nam có sự góp sức của  Phật   giáo . Điều đó giải thích tại sao “ Phật   giáo  từ hàng nghìn năm nay đã chiếm được một chỗ đứng khá vững chắc trong lòng của hàng triệu con người” và “có thể nói hai ngàn năm  Phật   giáo  Việt Nam là hai ngàn năm  Phật   giáo  nhập thân với dân tộc”, đúng như Chủ tịch Hồ Chí Minh nói: “ Phật   giáo  Việt Nam với dân tộc như hình với bóng, tuy hai mà một”.</p>", "/imgProduct/book2.jpg", "CUỘC VẬN ĐỘNG CỦA PHẬT GIÁO VIỆT NAM NĂM 1963", 56000m, 100, 2 },
                    { 3, "Lê Thước", 1, "<p>Tháng 6 năm 1921, cụ Lê Thước, Giải nguyên khoa thi Hương cuối cùng của triều Nguyễn tại trường thi Nghệ An năm 1918, đã trình bày bản luận văn bằng tiếng Pháp \"Về việc học chữ Hán\" trong kỳ thi tốt nghiệp khóa 2 trường Cao đẳng Sư phạm Đông Dương tại Hà Nội.</p><p>Bản luận văn này được viết trong bối cảnh cuộc cải cách giáo dục, dưới áp lực của chính phủ bảo hộ Pháp, đang diễn ra mạnh mẽ trên lãnh thổ Việt Nam. Giáo dục Việt Nam đang chuyển từ một nền giáo dục truyền thống theo mô hình Trung Hoa phong kiến cùng với chữ Hán sang nền giáo dục mới theo mô hình phương Tây cùng với chữ Pháp và chữ Quốc ngữ.</p><p>Trong luận văn này, cụ Lê Thước đã phân tích kỹ vai trò và ích lợi của chữ Hán đối với xã hội Việt Nam truyền thống trên các khía cạnh: lịch sử, nền tảng đạo đức, phong tục tập quán, ngôn ngữ và văn học, thực tiễn đời sống hàng ngày. Từ sự phân tích này, cụ Lê Thước đề xuất cần phải duy trì việc dạy chữ Hán cho học sinh phổ thông ở một mức độ nhất định để giúp họ hiểu rõ hơn tiếng Việt và nền văn hóa cổ truyền mà tổ tiên để lại.</p>", "/imgProduct/book3.jpg", "VỀ VIỆC HỌC CHỮ HÁN Ở VIỆT NAM", 30000m, 100, 2 },
                    { 4, "Trường ngoại ngữ Kanata - ThS. Lê Huy Khoa", 1, "<p>\"Nếu tính theo những trải nghiệm thì nghề phiên dịch mang lại rất nhiều trải nghiệm. Mỗi lần dịch sẽ là một kinh nghiệm hoàn toàn mới, mở rộng tầm nhìn, kiến thức, quan điểm sống, tiếp nhận nhiều thông tin mới, tăng cường các mối quan hệ mà khó ngành nào có thể so sánh được v.v...\".</p><p>Các bạn đang cầm trên tay quyển sách \"Cẩm nang kỹ năng nghề phiên dịch\" của tác giả Lê Huy Khoa, một người có kinh nghiệm làm công tác phiên dịch gần 30 năm trong rất nhiều lĩnh vực. Có thể nói đây là quyển sách duy nhất và đầy đủ nhất giới thiệu cho độc giả từ những tiêu chí, đặc tính của nghề phiên dịch, cho đến những nguyên tắc trong nghề, những kỹ năng cơ bản, phương pháp chuẩn bị một chương trình dịch, các bước chuẩn bị, hành trang cần trang bị, khắc phục các sự cố trong quá trình dịch, quá trình luyện tập để trở thành phiên dịch cũng như trả lời rất chi tiết tất cả các thắc mắc phát sinh trong quá trình phiên dịch, và đặc biệt là cả những trải nghiệm thú vị của cá nhân mà chỉ có nghề phiên dịch mới có thể có.</p>", "/imgProduct/book4.jpg", "CẨM NANG KỸ NĂNG NGHỀ PHIÊN DỊCH", 51000m, 100, 2 },
                    { 5, "Nguyễn Đức Hiệp", 2, "<p>\"Sự phát triển sân khấu nghệ thuật cải lương, kịch và điện ảnh cũng như âm nhạc có liên hệ mật thiết với nhau ở giai đoạn đầu và phát triển trong thế kỷ 20. Nhiều nghệ sĩ và soạn giả vừa hoạt động trên sân khấu cải lương và cũng tham gia đóng kịch, đóng phim và hát tân nhạc. Sự hình thành và phát triển của sân khấu hát lương từ hát bội và đờn ca tài tử truyền thống đã được nghiên cứu, trình bày và xuất bản trong các năm qua.</p><p>Quyển sách “Lịch sử sân khấu kịch và điện ảnh Việt Nam” ra đời với mục đích giới thiệu sự hình thành và phát triển của sân khấu kịch nghệ và điện ảnh vào đầu thế kỷ 20 cho đến thập niên 2010 chủ yếu ở Nam Bộ. Mặc dù chỉ giới hạn trong khoảng thời gian và không gian này nhưng chúng tôi cũng đề cập đến không gian rộng hơn trên toàn Việt Nam khi có sự tương tác và ảnh hưởng của các hình thái nghệ thuật trên ở các miền trước khi đất nước thống nhất. Phần tân nhạc, một lãnh vực âm nhạc có sự liên hệ nhiều đến nghệ thuật kịch và điện ảnh, chúng tôi sẽ viết trong một cuốn sách riêng về sự hình thành và phát triển tân nhạc Việt Nam vì lãnh vực này rất lớn và đa dạng.\"\r\n\r\n</p>", "/imgProduct/book5.jpg", "LỊCH SỬ SÂN KHẤU KỊCH VÀ ĐIỆN ẢNH VIỆT NAM", 11990000m, 100, 2 },
                    { 6, "GS. BS. Nguyễn Chấn Hùng", 2, "<p>Nhà xuất bản Tổng hợp Thành phố Hồ Chí Minh xin trân trọng giới thiệu tác phẩm “Sâu thẳm sự sống” của Giáo sư – Bác sĩ Nguyễn Chấn Hùng.</p><p>“Sâu thẳm sự sống” được ra mắt bạn đọc đầu tiên vào cuối năm 2010, từ một cuốn sách nhỏ tập hợp một số điều mà Bác sĩ Nguyễn Chấn Hùng gom góp, sau đó được bổ sung các thành tựu từ các giải Nobel mới qua các năm 2012 và 2015, cho đến năm 2022, sau 12 năm ra mắt lần đầu, tác phẩm lại lần nữa được bổ sung, nhờ có thêm thời gian và thêm nhiều giải Nobel mới hàng năm, tác giả đã học hiểu và cố gắng bố cục sách cho phù hợp hơn, để “sự sống được thêm sâu thẳm”.\r\nHơn nửa thế kỷ là thầy thuốc và thầy giáo, tác giả đã luôn cố gắng bắt nhịp cùng các thành tựu khoa học về sự sống và con người, đặc biệt lưu tâm đến các giải Nobel về Sinh lý hoặc Y học và một số giải Vật lý, Hóa học giúp tìm hiểu sự sống. Ông cũng luôn luôn trang bị kiến thức mới để “tinh nghề Y sâu nghề Giáo”, đặc biệt về bệnh ung thư chuyên ngành mà ông theo đuổi.</p><p>Giáo sư – Bác sĩ Nguyễn Chấn Hùng là một nhà khoa học với tâm hồn nghệ sĩ, ẩn giấu bên trong là vốn tri thức rộng và uyên bác về văn hoá, một người viết có trách nhiệm với khoa học và đông đảo quần chúng. Ông đã giúp cho mỗi người tìm thấy một thái độ sống tích cực mà ông hay gọi là “sống lành, sống kỹ”. Những bài viết của ông vào thứ hai hàng tuần trên Sài Gòn Tiếp Thị đã trở thành một cuộc hò hẹn được nhiều người mong chờ, một “món nợ” với độc giả khó mà dứt được. Món nợ nhân sinh mà tác giả đã tự nguyện đón nhận để chia sẻ với bạn đọc, giúp mọi người có được chìa khoá để khám phá cuộc sống xung quanh, vượt lên bệnh tật, làm chủ cuộc đời.\r\nQua \"Sâu Thẳm Sự Sống\", người đọc có thể hiểu được rằng với căn bệnh ung thư, 1/3 có thể phòng ngừa không cần điều trị. Với bố cục sắp xếp trật tự là các câu chuyện tưởng chừng đời thường nhưng chứa đựng nhiều thông tin lớn mà ông cất công tìm hiểu như: Ảo diệu dàn hòa tấu nội tiết, Mềm dẻo bộ óc, Hai giải Nobel ráp mí, Darwin – Mendel Tao phùng lịch sử, Cha đẻ của Cách mạng Xanh, Covid-19 cuộc chiến cam go nhất, Cái kéo di truyền CRISPR/Cas9 (giải Hóa học 2020), Các nữ anh hào Nobel, Lịch sử sự sống trên Trái đất… Cuối sách là phần Phụ lục về tác giả trích từ các bài báo, như: Một người lãng mạn cổ điển, Nụ cười luôn đồng hành, Trọn một đời cống hiến…</p>", "/imgProduct/book6.jpg", "SÂU THẲM SỰ SỐNG (TÁI BẢN 2023)", 112000m, 100, 2 },
                    { 7, "Nguyễn Thị Hậu", 2, "<p>Đi là để đến, để về, để thương những miền qua.\r\n\r\n“Thương những miền qua” là cuốn sách mới nhất của nhà nghiên cứu Nguyễn Thị Hậu xuất bản trong năm 2023.\r\n\r\nẤn tượng nhất sau khi đọc được từ sách của Nguyễn Thị Hậu là một giọng văn tự sự đậm chất Nam bộ, trong cách kể chuyện rủ rỉ tâm tình, gây nhớ, gây thương, và gây ngẫm ngợi… của người viết. Với tư thế và tâm thế của một phụ nữ làm nghề đặc biệt – Khảo cổ học, kể về những miền đất đi qua – với muôn vàn thương nhớ. Thực ra, Hậu không chỉ đi qua, mà là đi đến và cũng là đi về miền đất quê hương, nơi cha sinh mẹ đẻ. Và Hậu đã không ngừng đi, không chỉ đi, trên dọc dài đất nước mình. Để rồi, tất cả sự đi chứa chan cảm xúc ấy, đã lên hương, thành tình tự, ngụ trong một chữ thương, (theo cách nói Nam bộ, chữ thương dùng để chỉ chữ yêu. Con trai Nam bộ không nói anh yêu em mà nói anh thương em).\r\n\r\nBởi vậy, chữ thương ngự ngay tên sách của Hậu: Thương những miền qua.Vì thế, sách có chữ thương này rất có thể động lòng người chịu đọc nó. Bởi lẽ, người đọc nào mà chẳng thương miền đất đã sống, thương miền đất sắp đến và thương về miền đất mình sẽ trở lại - nơi quê hương nguồn cội.</p>", "/imgProduct/book7.jpg", "THƯƠNG NHỮNG MIỀN QUA (TÙY BÚT)", 41000m, 100, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_order_id",
                table: "Order_Items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_product_id",
                table: "Order_Items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_user_id",
                table: "Orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_category_id",
                table: "Products",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category_Requests");

            migrationBuilder.DropTable(
                name: "Order_Items");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
