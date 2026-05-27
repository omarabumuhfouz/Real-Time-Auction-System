import { NextResponse } from "next/server";
import fs from "fs";
import path from "path";
import crypto from "crypto";

export async function POST(request: Request) {
  try {
    const { searchParams } = new URL(request.url);
    const auctionId = searchParams.get("auctionId");

    if (!auctionId) {
      return NextResponse.json(
        { error: "Missing auctionId parameter" },
        { status: 400 }
      );
    }

    const formData = await request.formData();
    const files = formData.getAll("images") as File[];

    if (!files || files.length === 0) {
      return NextResponse.json(
        { error: "No files uploaded" },
        { status: 400 }
      );
    }

    // Base directory inside public to serve files statically
    const uploadDir = path.join(process.cwd(), "public", "uploads", "auctions", auctionId);

    // Create directory recursively if it doesn't exist
    if (!fs.existsSync(uploadDir)) {
      fs.mkdirSync(uploadDir, { recursive: true });
    }

    const savedUrls: string[] = [];

    for (const file of files) {
      const originalName = file.name;
      const ext = path.extname(originalName);
      const baseName = path.basename(originalName, ext);

      // Generate a unique GUID
      const guid = crypto.randomUUID();

      // Format name: itsname_GUID.type
      const newFileName = `${baseName}_${guid}${ext}`;
      const filePath = path.join(uploadDir, newFileName);

      // Convert File buffer to Node.js Buffer
      const arrayBuffer = await file.arrayBuffer();
      const buffer = Buffer.from(arrayBuffer);

      // Write file to filesystem
      fs.writeFileSync(filePath, buffer);

      // Construct dynamic public-facing serving URL
      const publicUrl = `/uploads/auctions/${auctionId}/${newFileName}`;
      savedUrls.push(publicUrl);
    }

    return NextResponse.json({ urls: savedUrls }, { status: 200 });
  } catch (error: any) {
    console.error("Error saving uploaded files:", error);
    return NextResponse.json(
      { error: "Failed to upload images", details: error.message },
      { status: 500 }
    );
  }
}

export async function DELETE(request: Request) {
  try {
    const { searchParams } = new URL(request.url);
    const auctionId = searchParams.get("auctionId");

    if (!auctionId) {
      return NextResponse.json(
        { error: "Missing auctionId parameter" },
        { status: 400 }
      );
    }

    // Base directory inside public to serve files statically
    const uploadDir = path.join(process.cwd(), "public", "uploads", "auctions", auctionId);

    // Delete directory and its contents recursively if it exists
    if (fs.existsSync(uploadDir)) {
      fs.rmSync(uploadDir, { recursive: true, force: true });
    }

    return NextResponse.json({ message: "Uploads cleaned up successfully" }, { status: 200 });
  } catch (error: any) {
    console.error("Error cleaning up uploaded files:", error);
    return NextResponse.json(
      { error: "Failed to cleanup images", details: error.message },
      { status: 500 }
    );
  }
}
